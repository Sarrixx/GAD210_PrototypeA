using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerminalApp : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Sprite icon;

    public string ID { get { return id; } }
    public Sprite Icon { get { return icon; } }

    public abstract bool StartApp(Terminal terminalInstance);
    public abstract bool Display(TerminalHUD hudInstance);
}
