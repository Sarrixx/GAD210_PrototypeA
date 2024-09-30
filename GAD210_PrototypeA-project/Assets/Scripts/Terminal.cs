using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Interactable, IPoweredObject
{
    [SerializeField] private float requiredPower = 50;

    public float RequiredPower { get { return requiredPower; } }
    public float ProvidedPower { get; private set; }
    public bool HasPower { get { return ProvidedPower >= requiredPower; } }

    public void PowerConnect(float powerAmount)
    {
        ProvidedPower += powerAmount;
    }

    public void PowerDisconnect(float powerAmount)
    {
        ProvidedPower -= powerAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

///setup terminals so that they run specific apps? e.g. a power network app that allows players to reroute power flow; an email app that can have mail configured
///on an instance basis; 
///or setup a series of inheritance-based terminals, with different terminal types?
///power terminals
///> reroute the flow of power
///> connect and disconnect devices
///> manage connected systems
///> manage power grids
///door access terminals
///> toggle lock state
///> toggle open/close
///> toggle active state
///security terminals
///> toggle alarms
///> toggle panels
///> toggle lasers