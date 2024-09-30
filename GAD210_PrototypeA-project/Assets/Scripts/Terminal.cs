using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Interactable, IPoweredEntity
{
    public delegate void TerminalEventDelegate(Terminal terminal);

    [SerializeField] private float requiredPower = 50;
    [SerializeField] private TerminalApp[] installedApps;
    [SerializeField] private string password;

    public float RequiredPower { get { return requiredPower; } }
    public float ProvidedPower { get; private set; }
    public bool HasPower { get { return ProvidedPower >= requiredPower; } }
    public TerminalApp[] InstalledApps { get { return installedApps; } }

    public static event TerminalEventDelegate OnTerminalEngagedEvent;
    public static event TerminalEventDelegate OnTerminalDisengagedEvent;

    public void PowerConnect(float powerAmount)
    {
        ProvidedPower += powerAmount;
    }

    public void PowerDisconnect(float powerAmount)
    {
        ProvidedPower -= powerAmount;
    }

    protected override void Awake()
    {
        base.Awake();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnInteract(out Interactable engagedAction)
    {
        engagedAction = null;
        if (HasPower == true)
        {
            if (base.OnInteract(out engagedAction) == true)
            {
                //trigger event OnInteractionEngaged to allow other components to react (like locking movement)
                OnTerminalEngagedEvent?.Invoke(this);
                engagedAction = this;
                return true;
            }
        }
        return false;
    }

    public override bool OnDisengageInteraction()
    {
        OnTerminalDisengagedEvent?.Invoke(null);
        //trigger event OnInteractionDisengaged to allow other components to react (like unlocking movement)
        return true;
    }
}