using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerminalApp : ScriptableObject
{
    [SerializeField] private Sprite icon;

    public Sprite Icon { get { return icon; } }

    public abstract bool Display(TerminalHUD hudInstance);
}
