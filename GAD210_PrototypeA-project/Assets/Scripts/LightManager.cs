using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private PowerLightGroup[] standardLighting;
    [SerializeField] private PowerLightGroup[] alarmLighting;

    private void Awake()
    {
        if (standardLighting.Length > 0 && alarmLighting.Length > 0)
        {
            foreach (PowerLightGroup lightGroup in standardLighting)
            {
                lightGroup.ActivateGroup(true);
                //if (lightingObject.activeSelf != true)
                //{
                //    lightingObject.SetActive(true);
                //}
            }
            foreach (PowerLightGroup lightGroup in alarmLighting)
            {
                lightGroup.ActivateGroup(false);
                //if (lightingObject.activeSelf != false)
                //{
                //    lightingObject.SetActive(false);
                //}
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BreachEvent += BreachResponse;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Debug.Log("Lighting manager responding to breach trigger.");
        if (standardLighting.Length > 0 && alarmLighting.Length > 0)
        {
            foreach (PowerLightGroup lightGroup in standardLighting)
            {
                lightGroup.ActivateGroup(false);
                //lightingObject.SetActive(false);
            }
            foreach (PowerLightGroup lightGroup in alarmLighting)
            {
                lightGroup.ActivateGroup(true);
                //lightingObject.SetActive(true);
            }
        }
    }
}
