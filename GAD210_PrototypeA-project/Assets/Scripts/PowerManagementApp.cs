using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power Manager App Instance", menuName = "Terminal Applications/Power Manager App", order  = 0)]
public class PowerManagementApp : TerminalApp
{
    ///> reroute the flow of power
    ///> connect and disconnect devices
    ///> manage connected systems
    ///> manage power grids
    public override bool Display(TerminalHUD hudInstance)
    {
        return hudInstance.DisplayApp(this);
    }

    public override bool StartApp(Terminal terminalInstance)
    {
        
        return false;
    }

    public bool ToggleGrid(PowerGrid powerGrid)
    {
        return false;
    }
}
