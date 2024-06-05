using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AlarmPanel : Interactable, IBreachTrigger
{
    /// <summary>
    /// Invoked when the alarm panel is interacted with.
    /// </summary>
    public event BreachTriggerDelegate BreachTriggerEvent = delegate { };

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        BreachTriggerEvent += GameManager.Instance.TriggerBreach;
        GameManager.Instance.BreachEvent += BreachResponse;
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Log("Alarm responding to breach event.");
        BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
    }

    /// <summary>
    /// Functionality executed when the alarm panel is interacted with.
    /// </summary>
    /// <param name="engagedAction"></param>
    /// <returns></returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        if (base.OnInteract(out engagedAction) == true)
        {
            BreachTriggerEvent.Invoke();
            return true;
        }
        return false;
    }
}
