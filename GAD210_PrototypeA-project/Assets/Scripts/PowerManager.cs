using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PowerManager : MonoBehaviour
{
    [Tooltip("Power grid definitions.")]
    [SerializeField] private PowerGrid[] powerGrids;

    /// <summary>
    /// Returns a reference to the PowerManager instance active in the scene.
    /// </summary>
    public static PowerManager Instance { get; private set; }

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        //instantiate singleton
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //initialise power grids
        for (int i = 0; i < powerGrids.Length; i++)
        {
            powerGrids[i] = new PowerGrid(powerGrids[i]);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        foreach(PowerGrid grid in powerGrids)
        {
            ActivateGrid(grid);
        }
    }

    /// <summary>
    /// Attempts to find a PowerGrid associated with the provided identifier and enable it.
    /// </summary>
    /// <param name="gridID">The identifier for the PowerGrid to be enabled.</param>
    /// <returns>Returns true if a PowerGrid associated with the provided identifier was successfully enabled.</returns>
    public bool ActivateGrid(string gridID)
    {
        foreach(PowerGrid grid in powerGrids)
        {
            if(grid.ID == gridID)
            {
                return ActivateGrid(grid);
            }
        }
        return false;
    }

    /// <summary>
    /// Enables the provided PowerGrid.
    /// </summary>
    /// <param name="grid">A reference to the PowerGrid to be enabled.</param>
    /// <returns>Returns false if the provided PowerGrid could not be enabled.</returns>
    public bool ActivateGrid(PowerGrid grid)
    {
        if (grid.ToggleGrid(true) == true)
        {
            Debug.Log("Activated power grid: " + grid.ID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to find a PowerGrid associated with the provided identifier and disable it.
    /// </summary>
    /// <param name="gridID">The identifier for the PowerGrid to be disabled.</param>
    /// <returns>Returns true if a PowerGrid associated with the provided identifier was successfully disabled.</returns>
    public bool DisableGrid(string gridID)
    {
        foreach (PowerGrid grid in powerGrids)
        {
            if (grid.ID == gridID)
            {
                return DisableGrid(grid);
            }
        }
        return false;
    }

    /// <summary>
    /// Disables the provided PowerGrid.
    /// </summary>
    /// <param name="grid">A reference to the PowerGrid to be disabled.</param>
    /// <returns>Returns false if the provided PowerGrid could not be disabled.</returns>
    public bool DisableGrid(PowerGrid grid)
    {
        if (grid.ToggleGrid(false) == true)
        {
            Debug.Log("Deactivated power grid: " + grid.ID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to get a PowerGrid instance with the provided identifier.
    /// </summary>
    /// <param name="gridID">The identifier associated with the PowerGrid being searched for.</param>
    /// <param name="grid">Stores and provides a reference to the PowerGrid instance if successfully found.</param>
    /// <returns>Returns true if a PowerGrid instance associated with the provided systemID was successfully located.</returns>
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
/// Hub for managing power systems on a power grid.
/// </summary>
[System.Serializable]
public class PowerGrid
{
    [Tooltip("The identifying string for the power grid.")]
    [SerializeField] private string id;
    [Tooltip("Definitions for the sub systems connected to the power grid.")]
    [SerializeField] private PowerSubSystem[] subSystems;

    /// <summary>
    /// The identifying string associated with this PowerGrid.
    /// </summary>
    public string ID { get { return id; } }
    /// <summary>
    /// Returns true if the PowerGrid is currently active and providing power to the connected PowerSubSystems.
    /// </summary>
    public bool Active { get; private set; } = false;

    /// <summary>
    /// Instantiate a PowerGrid, cloning the properties from an existing instance.
    /// </summary>
    /// <param name="instance">The PowerGrid instance to clone.</param>
    public PowerGrid(PowerGrid instance)
    {
        id = instance.ID;
        subSystems = instance.subSystems;
        //Automatically clone the subsystems defined in the original instance as well.
        for(int i = 0; i < subSystems.Length; i++)
        {
            subSystems[i] = new PowerSubSystem(subSystems[i]);
        }
    }

    /// <summary>
    /// Toggles the active state of this PowerGrid, affecting the active state of all connected PowerSubSystems.
    /// </summary>
    /// <param name="toggle">The value to change the active state of the PowerGrid to.</param>
    /// <returns>Returns false if the active state of the PowerGrid is already the same as the provided toggle value.</returns>
    public bool ToggleGrid(bool toggle)
    {
        if(Active == !toggle)
        {
            foreach (PowerSubSystem subSystem in subSystems)
            {
                if (subSystem.Active == !toggle)
                {
                    subSystem.ToggleSubsystem(toggle);
                }
            }
            Active = toggle;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to toggle the active state of a PowerSubSystem associated with the provided systemID that is connected to this PowerGrid.
    /// </summary>
    /// <param name="systemID">The identifier associated with the PowerSubSystem being searched for.</param>
    /// <param name="toggle">A value to change the active state of the PowerSubSystem to.</param>
    /// <returns>Returns true if the active state for the PowerSubSystem associated with the provuded systemID that is connected to the grid,
    /// was able to be toggled to the provided value.</returns>
    public bool ToggleSubSystem(string systemID, bool toggle)
    {
        for(int i = 0; i < subSystems.Length; i++)
        {
            if (subSystems[i].ID == systemID)
            {
                subSystems[i].ToggleSubsystem(toggle);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to get a PowerSubSystem instance connected to this PowerGrid with the provided identifier.
    /// </summary>
    /// <param name="systemID">The identifier associated with the PowerSubSystem instance being searched for.</param>
    /// <param name="system">Stores and provides a reference to the PowerSubSystem instance if successfully found.</param>
    /// <returns>Returns true if a PowerSubSystem instance associated with the provided systemID was successfully located.</returns>
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
    [Tooltip("An identifying string for the power subsystem.")]
    [SerializeField] private string id;
    [Tooltip("The maximum amount of power that can be provided for connected entities by the power subsystem.")]
    [SerializeField] private float powerLevel;
    [Tooltip("The entities that will be initially connected to the power subsystem when the scene starts.")]
    [SerializeField] private GameObject[] initialisedConnections;

    private float currentPowerUsage = 0;
    private readonly List<IPoweredEntity> connectedEntities = new List<IPoweredEntity>();

    /// <summary>
    /// Returns true if the PowerSubSystem is currently providing power to connected IPoweredEntitys.
    /// </summary>
    public bool Active { get; private set; } = false;
    /// <summary>
    /// Returns the remaining power available for this PowerSubSystem.
    /// </summary>
    private float AvailablePower { get { return powerLevel - currentPowerUsage; } }
    /// <summary>
    /// Returns the ID for the PowerSubSystem.
    /// </summary>
    public string ID { get { return id; } }

    /// <summary>
    /// Instantiate a PowerSubSystem, cloning the properties from an existing instance.
    /// </summary>
    /// <param name="instance">The PowerSubSystem instance to clone.</param>
    public PowerSubSystem(PowerSubSystem instance)
    {
        id = instance.id;
        powerLevel = instance.powerLevel;
        initialisedConnections = instance.initialisedConnections;
        //Automatically connect all initial entity connections.
        foreach(GameObject gameObject in initialisedConnections)
        {
            if(gameObject.TryGetComponent(out IPoweredEntity poweredInstance) == true)
            {
                Connect(poweredInstance);
            }
        }
    }

    /// <summary>
    /// Connects an IPoweredEntity to this PowerSubSystem.
    /// </summary>
    /// <param name="entity">The IPoweredEntity to be connected to this PowerSubSystem.</param>
    /// <returns>Returns true if the IPoweredEntity was successfully connected to this PowerSubSystem.</returns>
    public bool Connect(IPoweredEntity entity)
    {
        if (connectedEntities.Contains(entity) == false)
        {
            connectedEntities.Add(entity);
            if (Active == true)
            {
                if (AvailablePower >= 0)
                {
                    entity.PowerConnect(entity.RequiredPower);
                    currentPowerUsage += entity.RequiredPower;
                    Debug.Log($"Connected '{entity.GetType()}' to system: {id}");
                }
                else
                {
                    ToggleSubsystem(false);
                    Debug.Log($"System '{id}' overloaded. ({AvailablePower})");
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Completely disconnects an IPoweredEntity from this PowerSubSystem.
    /// </summary>
    /// <param name="entity">The IPoweredEntity to be disconnected from this PowerSubSystem.</param>
    /// <returns>Returns true if the IPoweredEntity was successfully disconnected from this PowerSubSystem.</returns>
    public bool Disconnect(IPoweredEntity entity)
    {
        if(connectedEntities.Contains(entity) == true)
        {
            connectedEntities.Remove(entity);
            entity.PowerDisconnect(entity.RequiredPower);
            currentPowerUsage -= entity.RequiredPower;
            Debug.Log($"Disconnected '{entity.GetType()}' from system: {id}");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Toggles the active state of this PowerSubSystem, affecting the power provided to all connected IPoweredEntitys.
    /// </summary>
    /// <param name="toggle">If true, the PowerSubSystem is activated and provides power to connected IPoweredEntitys.
    /// If false, the PowerSubSystem is deactivated and does not provide power to any connected IPoweredEntitys.</param>
    public void ToggleSubsystem(bool toggle)
    {
        if (Active != toggle)
        {
            foreach (IPoweredEntity connectedEntity in connectedEntities)
            {
                if (toggle == true)
                {
                    if (AvailablePower >= 0)
                    {
                        connectedEntity.PowerConnect(connectedEntity.RequiredPower);
                        currentPowerUsage += connectedEntity.RequiredPower;
                        Debug.Log($"Connected '{connectedEntity.GetType()}' to system: {id}");
                    }
                }
                else
                {
                    connectedEntity.PowerDisconnect(connectedEntity.RequiredPower);
                    currentPowerUsage -= connectedEntity.RequiredPower;
                }
            }
            Active = toggle;
            Debug.Log($"Power system '{id}' active state set to: {Active}");
        }
    }

    /// <summary>
    /// Toggles the power connection for an IPoweredEntity that is connected to this PowerSubSystem.
    /// </summary>
    /// <param name="entity">The IPoweredEntity to toggle the energy connection for.</param>
    /// <param name="toggle">If true, the provided IPoweredEntity will be provided energy from the PowerSubSystem;
    /// If false, the provided IPoweredEntity will not be provided energy from the PowerSubSystem.</param>
    /// <returns>Returns true if the provided IPoweredEntity is currently receiving power from the PowerSubSystem.</returns>
    public bool ToggleEntityProvidedPower(IPoweredEntity entity, bool toggle)
    {
        if (Active == true)
        {
            if (connectedEntities.Contains(entity) == true)
            {
                if (toggle == true)
                {
                    entity.PowerConnect(entity.RequiredPower);
                    currentPowerUsage += entity.RequiredPower;
                    Debug.Log($"Enabled '{entity.GetType()}'.");
                }
                else
                {
                    entity.PowerDisconnect(entity.RequiredPower);
                    currentPowerUsage -= entity.RequiredPower;
                    Debug.Log($"Disconnected '{entity.GetType()}'.");
                }
                return true;
            }
        }
        return false;
    }
}

public interface IPoweredEntity
{
    /// <summary>
    /// Returns the energy cost required to power the entity.
    /// </summary>
    public float RequiredPower { get; }
    /// <summary>
    /// Returns the amount of energy current provided to the entity.
    /// </summary>
    public float ProvidedPower { get; }
    /// <summary>
    /// Returns true if the entity's required power cost is currently satisfied.
    /// </summary>
    public bool HasPower { get { return ProvidedPower >= RequiredPower; } }

    /// <summary>
    /// Called when the entity is connected to a power grid.
    /// </summary>
    /// <param name="powerAmount">The amount of provided energy being added to this entity.</param>
    public void PowerConnect(float powerAmount);
    /// <summary>
    /// Called when the entity is disconnected from a power grid.
    /// </summary>
    /// <param name="powerAmount">The amount of provided energy being removed from this entity.</param>
    public void PowerDisconnect(float powerAmount);
}