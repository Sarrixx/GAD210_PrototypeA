using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLightGroup : MonoBehaviour, IPoweredEntity
{
    [SerializeField] private Light[] lights;
    [SerializeField] private float requiredPower;

    private bool active = false;

    public float RequiredPower { get { return requiredPower; } }
    public bool HasPower { get { return ProvidedPower >= requiredPower; } }

    public float ProvidedPower { get; private set; }

    public void PowerConnect(float powerAmount)
    {
        ProvidedPower += powerAmount;
        if(active == true)
        {
            ToggleLights(true);
        }
    }

    public void PowerDisconnect(float powerAmount)
    {
        ProvidedPower -= powerAmount;
        if (active == true)
        {
            ToggleLights(false);
        }
    }

    public void ActivateGroup(bool toggle)
    {
        active = toggle;
        if (active == true)
        {
            ToggleLights(true);
        }
        else
        {
            ToggleLights(false);
        }
    }

    private void ToggleLights(bool toggle)
    {
        foreach (Light light in lights)
        {
            if ((toggle == true && HasPower == false) == false)
            {
                light.enabled = toggle;
            }
        }
    }
}
