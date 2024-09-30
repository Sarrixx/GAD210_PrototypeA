using UnityEngine;

/// <summary>
/// Script responsible for door interaction functionality.
/// </summary>
public class DoorInteraction : Interactable, IBreachTrigger, ILoggable, IPoweredEntity
{
    [Header("Initialisers")]
    [Tooltip("Set to true to have the door open when the game starts. If true, must be enabled for all doors in lock group.")]
    [SerializeField] private bool openOnStart = false;
    [Tooltip("Set to true to lock the door when the game starts.")]
    [SerializeField] private bool lockedOnStart = false;
    [Header("Door Interaction")]
    [Tooltip("Include other doors that should lock/unlock when this door is locked/unlocked.")]
    [SerializeField] private DoorInteraction[] lockGroup;
    [Tooltip("The audio clip that plays on interaction while door is locked.")]
    [SerializeField] protected AudioClip lockedInteractionClip;
    [Header("Breach Response")]
    [Tooltip("Set to true to lock the door when a breach is triggered.")]
    [SerializeField] private bool lockedOnBreach = false;
    [SerializeField] private int maxLockedInteractions = 3;
    [Header("Power")]
    [SerializeField] private float requiredPower = 50;

    private bool locked = false;
    private int lockedInteractions = 0;
    private Animator anim;

    public float RequiredPower { get { return requiredPower; } }
    public float ProvidedPower { get; private set; }
    public bool HasPower { get { return ProvidedPower >= RequiredPower; } }
    public bool IsOpen { get { if (anim != null) { return anim.GetBool("open"); } else { return false; } } }

    /// <summary>
    /// Invoked when the player collides with the trigger.
    /// </summary>
    public event BreachTriggerDelegate BreachTriggerEvent = delegate { };

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    protected override void Awake()
    {
        if (transform.parent != null)
        {
            if (transform.parent.TryGetComponent(out anim) == false)
            {
                Log($"{transform.parent.name} requires an Animator component!", 1);
            }
            if (transform.parent.TryGetComponent(out aSrc) == false)
            {
                Log($"{transform.parent.name} requires an Audio Source component!", 1);
            }
        }
        else
        {
            Log($"{gameObject.name} requires a parent object.", 2);
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        BreachTriggerEvent += GameManager.Instance.TriggerBreach;
        GameManager.Instance.BreachEvent += BreachResponse;
        if (anim != null)
        {
            if (IsOpen == true && openOnStart == false)
            {
                anim.SetBool("open", false);
            }
            else
            {
                anim.SetBool("open", openOnStart);
            }
        }
        if(lockedOnStart == true)
        {
            ToggleLockState(true);
        }
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Log("Door responding to breach event.");
        BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
        if(lockedOnBreach == true && HasPower == true)
        {
            ToggleLockState(true);
        }
    }

    /// <summary>
    /// Executed when door is interacted with by the player.
    /// </summary>
    /// <param name="interactionInfo">The interaction data from the interaction system.</param>
    /// <param name="engagedAction">Outs a reference back to the interaction if it requires disengagement.</param>
    /// <returns>Returns true if the interaction was successfully completed.</returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        engagedAction = null;
        if (Active == true && HasPower == true)
        {
            if(ToggleDoorState() == false)
            {
                lockedInteractions++;
                if (lockedInteractions >= maxLockedInteractions)
                {
                    BreachTriggerEvent?.Invoke();
                }
            }
            if (disableOnSuccessfulInteraction == true)
            {
                Active = false;
                foreach (DoorInteraction door in lockGroup)
                {
                    if (door != this)
                    {
                        door.Active = false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Toggles the current door state between open and closed.
    /// </summary>
    public bool ToggleDoorState()
    {
        if (HasPower == true)
        {
            if (locked == false)
            {
                if (anim != null)
                {
                    if (IsOpen == false)
                    {
                        anim.SetBool("open", true);
                    }
                    else if (IsOpen == true)
                    {
                        anim.SetBool("open", false);
                    }
                }
                PlaySound(interactionClip, aSrc);
                return true;
            }
            else
            {
                PlaySound(lockedInteractionClip, aSrc);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Toggles the current lock state of the door and it's associated lock group.
    /// Door will also close if already open and lock state changed to true.
    /// </summary>
    /// <param name="lockState">If true, the door will be locked. If false, the door will be unlocked.</param>
    public bool ToggleLockState(bool lockState = true)
    {
        if (HasPower == true)
        {
            locked = lockState;
            foreach (DoorInteraction door in lockGroup)
            {
                if (door != this && door.locked != lockState)
                {
                    door.ToggleLockState(lockState);
                }
            }
            Log("Lockstate set to " + locked);
            if (locked == true)
            {
                if (anim != null)
                {
                    if (IsOpen == true)
                    {
                        anim.SetBool("open", false);
                        PlaySound(interactionClip, aSrc);
                    }
                }
                else
                {
                    PlaySound(interactionClip, aSrc);
                }
            }
            return true;
        }
        return false;
    }

    public void PowerConnect(float powerAmount)
    {
        ProvidedPower += powerAmount;
        Log("Power connected.");
    }

    public void PowerDisconnect(float powerAmount)
    {
        ProvidedPower -= powerAmount;
        Log("Power disconnected.");
    }
}
