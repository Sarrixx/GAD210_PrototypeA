using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power Manager App Instance", menuName = "Terminal Applications/Power Manager App", order  = 0)]
public class PowerManagementApp : TerminalApp
{
    [SerializeField] private string[] gridLabels;

    public List<string> SubSystemLabels 
    { 
        get
        {
            List<string> systemLabels = new List<string>();
            foreach(string label in gridLabels)
            {
                string[] labels = label.Split('_');
                labels[0] = labels[0].Replace("_", string.Empty);
                if (labels.Length > 1)
                {
                    systemLabels.Add(label);
                }
                else
                {
                    continue;
                }
            }
            return systemLabels;
        } 
    }

    ///> manage connected systems

    public bool SubSystemConnectEntity(IPoweredEntity entity, PowerSubSystem subSystemInstance)
    {
        return false;
    }

    public bool SubSystemDisconnectEntity(IPoweredEntity entity, PowerSubSystem subSystemInstance)
    {
        return false;
    }
    
    
    public override bool Display(TerminalHUD hudInstance)
    {
        return hudInstance.DisplayApp(this);
    }

    public bool ToggleGrid(PowerGrid powerGrid)
    {
        if (PowerManager.Instance != null)
        {
            for(int i = 0; i < gridLabels.Length; i++)
            {
                if (gridLabels[i] == powerGrid.ID) 
                {
                    powerGrid.ToggleGrid(!powerGrid.Active);
                    return true;
                }
            }
        }
        return false;
    }

    public bool GetGrid(int index, out PowerGrid grid)
    {
        grid = null;
        if (index >= 0 && index < gridLabels.Length)
        {
            if (PowerManager.Instance != null)
            {
                return PowerManager.Instance.GetGrid(gridLabels[index], out grid);
            }
        }
        return false;
    }

    public bool GetPowerSubSystem(int index, out PowerSubSystem powerSubSystem)
    {
        powerSubSystem = null;
        if (index >= 0 && index < gridLabels.Length)
        {
            if (PowerManager.Instance != null)
            {
                return PowerManager.Instance.TryGetSubSystem(gridLabels[index], out powerSubSystem);
            }
        }
        return false;
    }

    public bool GetConnectedEntity(PowerSubSystem system, int index, out IPoweredEntity entity)
    {
        entity = null;
        if (system != null && index >= 0 && index < system.ConnectedEntities.Count)
        {
            if (PowerManager.Instance != null)
            {
                entity = system.ConnectedEntities[index];
                return true;
            }
        }
        return false;
    }
}
