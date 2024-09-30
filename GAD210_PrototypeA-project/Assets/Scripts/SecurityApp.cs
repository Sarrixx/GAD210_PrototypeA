using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Security App Instance", menuName = "Terminal Applications/Security App", order = 0)]
public class SecurityApp : TerminalApp
{
    ///security terminals
    ///> toggle alarms
    ///> override active lasers
    public override bool Display(TerminalHUD hudInstance)
    {
        return hudInstance.DisplayApp(this);
    }

    public override bool StartApp(Terminal terminalInstance)
    {
        throw new System.NotImplementedException();
    }

    public bool LockDoor(DoorInteraction doorInstance)
    {
        if(doorInstance != null)
        {
            return doorInstance.ToggleLockState(true);
        }
        return false;
    }

    public bool UnlockDoor(DoorInteraction doorInstance)
    {
        if(doorInstance != null)
        {
            return doorInstance.ToggleLockState(false);
        }
        return false;
    }

    public bool OpenDoor(DoorInteraction doorInstance)
    {
        if (doorInstance != null && doorInstance.IsOpen == false)
        {
            return doorInstance.ToggleDoorState();
        }
        return false;
    }

    public bool CloseDoor(DoorInteraction doorInstance)
    {
        if (doorInstance != null && doorInstance.IsOpen == true)
        {
            return doorInstance.ToggleDoorState();
        }
        return false;
    }
}
