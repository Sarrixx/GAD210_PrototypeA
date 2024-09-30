using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalHUD : MonoBehaviour
{
    [SerializeField] private Canvas mailAppCanvas;
    [SerializeField] private Canvas powerAppCanvas;
    [SerializeField] private Canvas securityAppCanvas;
    [SerializeField] private Image[] iconImages;
    [SerializeField] private Text mailMessageText;

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
            if (mailAppCanvas != null)
            {
                mailAppCanvas.enabled = false;
            }
            if (powerAppCanvas != null)
            {
                powerAppCanvas.enabled = false;
            }
            if (securityAppCanvas != null)
            {
                securityAppCanvas.enabled = false;
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
                if (mailAppCanvas != null)
                {
                    mailAppCanvas.enabled = false;
                }
                if (powerAppCanvas != null)
                {
                    powerAppCanvas.enabled = false;
                }
                if (securityAppCanvas != null)
                {
                    securityAppCanvas.enabled = false;
                }
                return true;
            case MailApp mail:
                if (mailAppCanvas != null)
                {
                    mailAppCanvas.enabled = true;
                    mail.SelectMail(0, out Mail? message);
                    if (message is Mail _message)
                    {
                        DisplayMail(_message);
                    }
                }
                if (powerAppCanvas != null)
                {
                    powerAppCanvas.enabled = false;
                }
                if (securityAppCanvas != null)
                {
                    securityAppCanvas.enabled = false;
                }
                return true;
            case SecurityApp security:
                if (mailAppCanvas != null)
                {
                    mailAppCanvas.enabled = false;
                }
                if (powerAppCanvas != null)
                {
                    powerAppCanvas.enabled = false;
                }
                if (securityAppCanvas != null)
                {
                    securityAppCanvas.enabled = true;
                }
                return true;
            case PowerManagementApp power:
                if (mailAppCanvas != null)
                {
                    mailAppCanvas.enabled = false;
                }
                if (powerAppCanvas != null)
                {
                    powerAppCanvas.enabled = true;
                }
                if (securityAppCanvas != null)
                {
                    securityAppCanvas.enabled = false;
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

    private void DisplayMail(Mail mail)
    {
        mailMessageText.text = mail.CompiledMessage;
    }
}

public abstract class TerminalAppHUD
{
    public abstract void Open();
    public abstract void Close();
}

public class MailAppHUD : TerminalAppHUD
{
    public override void Close()
    {
        throw new System.NotImplementedException();
    }

    public override void Open()
    {
        throw new System.NotImplementedException();
    }
}