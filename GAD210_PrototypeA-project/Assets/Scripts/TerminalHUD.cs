using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class TerminalHUD : MonoBehaviour
{
    [SerializeField] private Image[] iconImages;

    [SerializeField] private MailAppUI mailAppUI;
    [SerializeField] private SecurityAppUI securityAppUI;
    [SerializeField] private PowerAppUI powerAppUI;

    private Canvas terminalCanvas;
    private Terminal currentTerminal;
    private int selectedAppIndex;

    private void Awake()
    {
        TryGetComponent(out terminalCanvas);
        Terminal.OnTerminalEngagedEvent += ToggleTerminalDisplay;
        Terminal.OnTerminalDisengagedEvent += ToggleTerminalDisplay;
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleTerminalDisplay(null);
    }

    private void ToggleTerminalDisplay(Terminal terminal)
    {
        if (terminal != null)
        {
            if (terminal.HasPower == true && terminal.Active == true)
            {
                currentTerminal = terminal;
                DisplayDesktop();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else if (terminalCanvas.enabled == true)
        {
            terminalCanvas.enabled = false;
            currentTerminal = null;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void DisplayDesktop()
    {
        if (currentTerminal != null)
        {
            terminalCanvas.enabled = true;
            if (mailAppUI.Active == true)
            {
                mailAppUI.Close();
            }
            if (securityAppUI.Active == true)
            {
                securityAppUI.Close();
            }
            if (powerAppUI.Active == true)
            {
                powerAppUI.Close();
            }

            for (int i = 0; i < iconImages.Length; i++)
            {
                if (i < currentTerminal.InstalledApps.Length)
                {
                    iconImages[i].sprite = currentTerminal.InstalledApps[i].Icon;
                    iconImages[i].gameObject.SetActive(true);
                }
                else
                {
                    iconImages[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public bool DisplayApp<TTerminalApp>(TTerminalApp app) where TTerminalApp : TerminalApp
    {
        switch (app)
        {
            case null:
                if (mailAppUI.Active == true)
                {
                    mailAppUI.Close();
                }
                if (securityAppUI.Active == true)
                {
                    securityAppUI.Close();
                }
                if (powerAppUI.Active == true)
                {
                    powerAppUI.Close();
                }
                return true;
            case MailApp mail:
                if (mailAppUI.Active == false)
                {
                    mailAppUI.Open(mail);
                }
                if (securityAppUI.Active == true)
                {
                    securityAppUI.Close();
                }
                if (powerAppUI.Active == true)
                {
                    powerAppUI.Close();
                }
                return true;
            case SecurityApp security:
                if (mailAppUI.Active == true)
                {
                    mailAppUI.Close();
                }
                if (securityAppUI.Active == false)
                {
                    securityAppUI.Open();
                }
                if (powerAppUI.Active == true)
                {
                    powerAppUI.Close();
                }
                return true;
            case PowerManagementApp power:
                if (mailAppUI.Active == true)
                {
                    mailAppUI.Close();
                }
                if (securityAppUI.Active == true)
                {
                    securityAppUI.Close();
                }
                if (powerAppUI.Active == false)
                {
                    powerAppUI.Open(power);
                }
                return true;
        }
        return false;
    }

    public void SelectApp(int index)
    {
        if (currentTerminal != null)
        {
            if (index < currentTerminal.InstalledApps.Length)
            {
                selectedAppIndex = index;
                currentTerminal.InstalledApps[selectedAppIndex].Display(this);
            }
        }
    }

    public void CloseCurrentApp()
    {
        if(currentTerminal != null)
        {
            if (selectedAppIndex < currentTerminal.InstalledApps.Length)
            {
                DisplayApp<TerminalApp>(null);
            }
        }
    }

    public void SelectInboxMessage(int index)
    {
        if(currentTerminal != null)
        {
            if (currentTerminal.InstalledApps[selectedAppIndex] is MailApp mail)
            {
                mailAppUI.SelectInboxMessage(mail, index);
            }
        }
    }

    public void SelectPowerGrid(int index)
    {
        if(currentTerminal != null)
        {
            if (currentTerminal.InstalledApps[selectedAppIndex] is PowerManagementApp powerManager)
            {
                powerAppUI.SelectGrid(powerManager, index);
            }
        }
    }

    public void SelectSubSystemGrid(int index)
    {
        if(currentTerminal != null)
        {
            if (currentTerminal.InstalledApps[selectedAppIndex] is PowerManagementApp powerManager)
            {
                powerAppUI.SelectSubSystem(powerManager, index);
            }
        }
    }

    public void SelectConnectedEntity()
    {
        if (currentTerminal != null)
        {
            if (currentTerminal.InstalledApps[selectedAppIndex] is PowerManagementApp powerManager)
            {
                powerAppUI.UpdateSelectedEntityInfo(powerManager);
            }
        }
    }
}

public abstract class TerminalAppUI
{
    [SerializeField] protected Canvas appCanvas;

    public bool Active { get { return appCanvas.enabled; } }

    public virtual void Open()
    {
        appCanvas.enabled = true;
    }
    public virtual void Close()
    {
        appCanvas.enabled = false;
    }
}

[System.Serializable]
public class MailAppUI : TerminalAppUI
{
    [System.Serializable]
    private struct MailAppInboxMessage
    {
        [SerializeField] private Image image;
        [SerializeField] private Text text;

        public Image Image { get { return image;} }
        public Text Text { get { return text;} }
    }

    [SerializeField] private Text mailMessageText;
    [SerializeField] MailAppInboxMessage[] inboxMessages;

    public void Open(MailApp appInstance)
    {
        base.Open();
        for(int i = 0; i < inboxMessages.Length; i++)
        {
            if(i < appInstance.Inbox.Length)
            {
                inboxMessages[i].Image.gameObject.SetActive(true);
                inboxMessages[i].Image.color = Color.white;
                inboxMessages[i].Text.text = $"{appInstance.Inbox[i].SubjectLine}\nFrom: {appInstance.Inbox[i].FromAddress}";
            }
            else
            {
                inboxMessages[i].Image.gameObject.SetActive(false);
            }
        }
        SelectInboxMessage(appInstance, 0);
    }

    public void SelectInboxMessage(MailApp appInstance, int index)
    {
        if (appInstance.SelectMail(index, out Mail message) == true)
        {
            inboxMessages[index].Image.color = Color.grey;
            for (int i = 0; i < inboxMessages.Length; i++)
            {
                if(i != index)
                {
                    inboxMessages[i].Image.color = Color.white;
                }
            }
            DisplayMail(message);
        }
    }

    private void DisplayMail(Mail mail)
    {
        mailMessageText.text = mail.CompiledMessage;
    }
}

[System.Serializable]
public class PowerAppUI : TerminalAppUI
{
    [System.Serializable]
    private struct GridSelectionUIData
    {
        [SerializeField] private Image gridIconImage;
        [SerializeField] private Text gridText;

        public Image GridIconImage { get { return gridIconImage; } }
        public Text GridText { get { return gridText; } }
    }

    [System.Serializable]
    private struct SubSystemSelectionUIData
    {
        [SerializeField] private Image subSystemOptionImage;
        [SerializeField] private Text subSystemOptionText;

        public Image SubSystemOptionImage { get { return subSystemOptionImage; } }
        public Text SubSystemOptionText { get { return subSystemOptionText; } }
    }

    [SerializeField] private Image gridSelectionPanel;
    [SerializeField] private GridSelectionUIData[] gridSelectionOptions;
    [SerializeField] private Image subSystemSelectionPanel;
    [SerializeField] private SubSystemSelectionUIData[] subSystemSelectionOptions;
    [SerializeField] private Image entityInfoPanel;
    [SerializeField] private Text entityInfoText;
    [SerializeField] private Dropdown systemEntitiesDropdown;

    private PowerGrid selectedGrid;
    private PowerSubSystem selectedSubSystem;

    public void Open(PowerManagementApp appInstance)
    {
        base.Open();
        ToggleSubSystemSelectionPanel(appInstance, false);
        UpdateSelectedEntityInfo(appInstance);
    }

    public override void Close()
    {
        base.Close();
        selectedGrid = null;
        selectedSubSystem = null;
    }

    public void SelectGrid(PowerManagementApp appInstance, int index)
    {
        if(appInstance.GetGrid(index, out selectedGrid) == true)
        {
            gridSelectionOptions[index].GridIconImage.color = Color.grey;
            for (int i = 0; i < gridSelectionOptions.Length; i++)
            {
                if (i != index)
                {
                    gridSelectionOptions[i].GridIconImage.color = Color.white;
                }
            }
            ToggleSubSystemSelectionPanel(appInstance, true);
        }
    }

    public void SelectSubSystem(PowerManagementApp appInstance, int index)
    {
        if(appInstance.GetPowerSubSystem(index, out selectedSubSystem) == true)
        {
            //set sub system entity dropdown options
            systemEntitiesDropdown.ClearOptions();
            List<string> dropdownOptions = new List<string>(selectedSubSystem.ConnectedEntities.Count);
            for (int i = 0; i < selectedSubSystem.ConnectedEntities.Count; i++)
            {
                dropdownOptions.Add(selectedSubSystem.ConnectedEntities[i].GetType().ToString().ToUpper());
            }
            systemEntitiesDropdown.AddOptions(dropdownOptions);
            if (systemEntitiesDropdown.gameObject.activeSelf == false)
            {
                systemEntitiesDropdown.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateSelectedEntityInfo(PowerManagementApp appInstance)
    {
        if(appInstance.GetConnectedEntity(selectedSubSystem, systemEntitiesDropdown.value, out IPoweredEntity entity) == true)
        {
            entityInfoPanel.gameObject.SetActive(true);
            string entityInfo = $"{entity.GetType()}";
            //connected grids?
            //connected sub systems?
            entityInfo += $"\n{entity.ProvidedPower}/{entity.RequiredPower}";
            entityInfoText.text = entityInfo;
        }
        else
        {
            entityInfoText.text = "";
            entityInfoPanel.gameObject.SetActive(false);
        }
    }

    private void ToggleSubSystemSelectionPanel(PowerManagementApp appInstance, bool toggle)
    {
        subSystemSelectionPanel.gameObject.SetActive(toggle);
        //UpdateSelectedEntityInfo(null);
        if (toggle == true && selectedGrid != null)
        {
            if (systemEntitiesDropdown.gameObject.activeSelf == true)
            {
                systemEntitiesDropdown.gameObject.SetActive(false);
            }
            //set sub system options
            List<string> systemLabels = appInstance.SubSystemLabels;
            for (int i = 0; i < subSystemSelectionOptions.Length; i++)
            {
                if (i < systemLabels.Count)
                {
                    subSystemSelectionOptions[i].SubSystemOptionImage.gameObject.SetActive(true);
                    subSystemSelectionOptions[i].SubSystemOptionText.text = systemLabels[i];
                }
                else
                {
                    subSystemSelectionOptions[i].SubSystemOptionImage.gameObject.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class SecurityAppUI : TerminalAppUI
{

}