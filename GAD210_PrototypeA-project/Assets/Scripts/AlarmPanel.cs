using UnityEngine;

public class AlarmPanel : Interactable, IBreachTrigger, IPoweredObject
{
    [SerializeField] private float requiredPower = 50;
    [SerializeField] private AudioSource alarmSource;

    public float RequiredPower { get { return requiredPower; } }

    public float ProvidedPower { get; private set; }

    public bool HasPower { get { return ProvidedPower >= requiredPower; } }

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
        if (alarmSource != null && alarmSource.clip != null)
        {
            alarmSource.loop = true;
            alarmSource.Play();
        }
    }

    /// <summary>
    /// Functionality executed when the alarm panel is interacted with.
    /// </summary>
    /// <param name="engagedAction"></param>
    /// <returns></returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        if (base.OnInteract(out engagedAction) == true && HasPower == true)
        {
            BreachTriggerEvent.Invoke();
            return true;
        }
        return false;
    }

    public void PowerConnect(float powerAmount)
    {
        ProvidedPower += powerAmount;
        //enable digital display
        Log("Power connected.");
    }

    public void PowerDisconnect(float powerAmount)
    {
        ProvidedPower -= powerAmount;
        //disable digital display
        Log("Power disconnected.");
    }
}
