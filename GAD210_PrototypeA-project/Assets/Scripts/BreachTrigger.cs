using UnityEngine;

public class BreachTrigger : MonoBehaviour, IBreachTrigger, ILoggable
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("If true, the breach will be invoked when the player leaves the trigger, rather than when they enter it.")]
    [SerializeField] private bool triggerOnExit = false;

    /// <summary>
    /// Invoked when the player collides with the trigger.
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
    /// Executed when the player enters the trigger collider.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(triggerOnExit == false && other.CompareTag("Player") == true)
        {
            BreachTriggerEvent?.Invoke();
        }
    }

    /// <summary>
    /// Executed when the player exits the trigger collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (triggerOnExit == true && other.CompareTag("Player") == true)
        {
            BreachTriggerEvent?.Invoke();
        }
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Log("Breach trigger responding to breach event.");
        BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [BREACH TRIGGER] and the name of the associated game objected concatenated as a prefix.</param>
    /// <param name="level">A level of 0 prints a standard message.
    /// A level of 1 prints a warning message.
    /// A level of 2 prints an error message.</param>
    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[BREACH TRIGGER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[BREACH TRIGGER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[BREACH TRIGGER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}
