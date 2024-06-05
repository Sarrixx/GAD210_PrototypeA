using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("The points the turret will move between.")]
    [SerializeField] private Transform[] points;
    [Tooltip("The time taken to move between points.")]
    [SerializeField] private float timeToPoint = 3;

    private int behaviour = 0;
    private int currentPoint = 0;
    private float timer = -1;
    private bool active = false;
    private Vector3 startPos;

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
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if(active == true && points.Length >= 2)
        {
            if(Vector3.Distance(transform.position, points[currentPoint].position) <= 0)
            {
                if (behaviour > 0)
                {
                    if (points.Length > currentPoint + 1)
                    {
                        currentPoint++;
                        startPos = transform.position;
                        timer = 0;
                    }
                    else
                    {
                        behaviour = -1;
                    }
                }
                else if(behaviour < 0)
                {
                    if (currentPoint > 0)
                    {
                        currentPoint--;
                        startPos = transform.position;
                        timer = 0;
                    }
                    else
                    {
                        behaviour = 1;
                    }
                }
            }
            else if(timer >= 0 && behaviour != 0)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, points[currentPoint].position, timer / timeToPoint);
            }
        }
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Debug.Log("Laser responding to breach event.");
        BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
        active = true;
        startPos = transform.position;
        timer = 0;
        behaviour = 1;
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
                    Debug.Log($"[LASER TURRET] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[LASER TURRET] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[LASER TURRET] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}
