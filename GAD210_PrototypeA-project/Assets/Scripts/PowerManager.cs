using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [SerializeField] private PowerGrid[] powerGrids;

    public static PowerManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        for (int i = 0; i < powerGrids.Length; i++)
        {
            powerGrids[i] = new PowerGrid(powerGrids[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(PowerGrid grid in powerGrids)
        {
            ActivateGrid(grid);
        }
    }

    public bool ActivateGrid(string gridID)
    {
        foreach(PowerGrid grid in powerGrids)
        {
            if(grid.ID == gridID)
            {
                if(grid.Toggle(true) == true)
                {
                    Debug.Log("Activated power grid: " + gridID);
                    return true;
                }
            }
        }
        return false;
    }

    public bool ActivateGrid(PowerGrid grid)
    {
        if (grid.Toggle(true) == true)
        {
            Debug.Log("Activated power grid: " + grid.ID);
            return true;
        }
        return false;
    }

    public bool DisableGrid(string gridID)
    {
        foreach (PowerGrid grid in powerGrids)
        {
            if (grid.ID == gridID)
            {
                if (grid.Toggle(false) == true)
                {
                    Debug.Log("Deactivated power grid: " + gridID);
                    return true;
                }
            }
        }
        return false;
    }

    public bool DisableGrid(PowerGrid grid)
    {
        if (grid.Toggle(false) == true)
        {
            Debug.Log("Deactivated power grid: " + grid.ID);
            return true;
        }
        return false;
    }

    public bool GetGrid(string gridID, out PowerGrid grid)
    {
        grid = null;
        for (int i = 0; i < powerGrids.Length; i++)
        {
            if (powerGrids[i].ID == gridID) 
            { 
                grid = powerGrids[i];
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Manages power systems.
/// </summary>
[System.Serializable]
public class PowerGrid
{
    [SerializeField] private string id;
    [SerializeField] private PowerSubSystem[] subSystems;

    public string ID { get { return id; } }
    public bool Active { get; private set; } = false;

    public PowerGrid(PowerGrid instance)
    {
        id = instance.ID;
        subSystems = instance.subSystems;
        for(int i = 0; i < subSystems.Length; i++)
        {
            subSystems[i] = new PowerSubSystem(subSystems[i]);
        }
    }

    public bool Toggle(bool toggle)
    {
        if(Active == !toggle)
        {
            foreach (PowerSubSystem subSystem in subSystems)
            {
                if (subSystem.Active == !toggle)
                {
                    subSystem.Toggle(toggle);
                }
            }
            Active = toggle;
            return true;
        }
        return false;
    }

    public bool ToggleSubSystem(string systemID, bool toggle)
    {
        for(int i = 0; i < subSystems.Length; i++)
        {
            if (subSystems[i].ID == systemID)
            {
                subSystems[i].Toggle(toggle);
                return true;
            }
        }
        return false;
    }

    public bool GetSubSystem(string systemID, out PowerSubSystem system)
    {
        system = null;
        for (int i = 0; i < subSystems.Length; i++)
        {
            if (subSystems[i].ID == systemID)
            {
                system = subSystems[i];
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Manages the distribution of available power between connected objects.
/// </summary>
[System.Serializable]
public class PowerSubSystem
{
    [SerializeField] private string id;
    [SerializeField] private float powerLevel;
    [SerializeField] private GameObject[] initialisedConnections;

    private float currentPowerUsage = 0;
    private List<IPoweredObject> connectedObjects = new List<IPoweredObject>();

    public bool Active { get; private set; } = false;
    private float AvailablePower { get { return powerLevel - currentPowerUsage; } }
    public string ID { get { return id; } }

    public PowerSubSystem(PowerSubSystem instance)
    {
        id = instance.id;
        powerLevel = instance.powerLevel;
        initialisedConnections = instance.initialisedConnections;
        foreach(GameObject gameObject in initialisedConnections)
        {
            if(gameObject.TryGetComponent(out IPoweredObject poweredInstance) == true)
            {
                Connect(poweredInstance);
            }
        }
    }

    public bool Connect(IPoweredObject entity)
    {
        if (connectedObjects.Contains(entity) == false)
        {
            connectedObjects.Add(entity);
            currentPowerUsage += entity.RequiredPower;
            if (Active == true)
            {
                if (AvailablePower >= 0)
                {
                    entity.PowerConnect(entity.RequiredPower);
                    Debug.Log($"Connected '{entity.GetType()}' to system: {id}");
                }
                else
                {
                    Toggle(false);
                    Debug.Log($"System '{id}' overloaded. ({AvailablePower})");
                }
            }
            return true;
        }
        return false;
    }

    public bool Disconnect(IPoweredObject entity)
    {
        if(connectedObjects.Contains(entity) == true)
        {
            connectedObjects.Remove(entity);
            currentPowerUsage -= entity.RequiredPower;
            entity.PowerDisconnect(entity.RequiredPower);
            Debug.Log($"Disconnected '{entity.GetType()}' from system: {id}");
            return true;
        }
        return false;
    }

    public void Toggle(bool toggle)
    {
        if (Active != toggle)
        {
            foreach (IPoweredObject connectedObject in connectedObjects)
            {
                //if (connectedObject.HasPower == !toggle)
                //{
                    if (toggle == true)
                    {
                        if (AvailablePower >= 0)
                        {
                            connectedObject.PowerConnect(connectedObject.RequiredPower);
                            Debug.Log($"Connected '{connectedObject.GetType()}' to system: {id}");
                        }
                    }
                    else
                    {
                        connectedObject.PowerDisconnect(connectedObject.RequiredPower);
                    }
                //}
            }
            Active = toggle;
            Debug.Log($"Power system '{id}' active state set to: {Active}");
        }
    }

    public bool ToggleObject(IPoweredObject entity, bool toggle)
    {
        if (Active == true)
        {
            if (connectedObjects.Contains(entity) == true)
            {
                if (toggle == true)
                {
                    entity.PowerConnect(entity.RequiredPower);
                    Debug.Log($"Enabled '{entity.GetType()}'.");
                }
                else
                {
                    entity.PowerDisconnect(entity.RequiredPower);
                    Debug.Log($"Disconnected '{entity.GetType()}'.");
                }
            }
            return true;
        }
        return false;
    }
}

public interface IPoweredObject
{
    public float RequiredPower { get; }
    public float ProvidedPower { get; }
    public bool HasPower { get { return ProvidedPower >= RequiredPower; } }

    public void PowerConnect(float powerAmount);
    public void PowerDisconnect(float powerAmount);
}