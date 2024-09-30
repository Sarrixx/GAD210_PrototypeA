using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGridSwitch : Interactable
{
    [SerializeField] private string[] gridLabels;

    /// <summary>
    /// Functionality executed when the alarm panel is interacted with.
    /// </summary>
    /// <param name="engagedAction"></param>
    /// <returns></returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        if (base.OnInteract(out engagedAction) == true)
        {
            if(PowerManager.Instance != null)
            {
                foreach (string id in gridLabels)
                {
                    string[] ids = id.Split('_');
                    if (PowerManager.Instance.GetGrid(ids[0].Replace("_", string.Empty), out PowerGrid grid) == true)
                    {
                        if (ids.Length == 1)
                        {
                            grid.ToggleGrid(!grid.Active);
                        }
                        else if (grid.GetSubSystem(ids[1].Replace("_", string.Empty), out PowerSubSystem system) == true)
                        {
                            system.ToggleSubsystem(!system.Active);
                        }
                    }
                    else
                    {
                        if (debug == true)
                        {
                            Log($"Power grid with ID '{id}' not found!");
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
