using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SecurityCamera : MonoBehaviour
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("The amount of time the camera will idle for between rotations.")]
    [SerializeField] private float idleTime = 3f;
    [Tooltip("The amount of time the camera takes to rotate.")]
    [SerializeField] private float rotationTime = 5f;
    [Tooltip("The limit to the angle the camera rotates from its forward direction.")]
    [SerializeField] private float rotationClamp = 30f;
    //[Tooltip("How far the camera can see.")]
    //[SerializeField] private float viewDistance = 10f;
    //[Tooltip("The radius of the camera's line of sight.")]
    //[SerializeField] private float detectionRadius = 0.5f;
    //[Tooltip("The offset for the second line of sight ray from the first ray.")]
    //[SerializeField] [Range(0, 1)] private float rayOffset = 0.15f;
    [Tooltip("Defines the angle for the camera's line of sight.")]
    [SerializeField][Range(5f, 180f)] private float lineOfSightAngle = 45f;
    [Tooltip("Defines the distance for the camera's line of sight.")]
    [SerializeField][Range(1f, 100f)] private float lineOfSightDistance = 10f;
    [Tooltip("The amount of time the player can be seen before the camera is aware.")]
    [SerializeField] private float awareTime = 1.5f;
    [Tooltip("Determines the camera's starting (and consequent following) behaviour. 0 is a static camera. -1 and 1 causes the camera to start rotating to either side.")]
    [SerializeField][Range(-1, 1)] private int behaviour = 0;
    [Tooltip("Where the player will respawn when detected by this camera.")]
    [SerializeField] private Transform respawnPoint;
    [Tooltip("A reference to the alert fill image for the camera.")]
    [SerializeField] private Image fillImage;

    private float idleTimer = -1;
    private float rotationTimer = -1;
    private float awareTimer = -1;
    private Vector3 originalRotation;
    private Quaternion startRotation;
    private Quaternion targetRotation;

    /// <summary>
    /// Tracks the camera's current target within range.
    /// </summary>
    public Transform Target { get; private set; }
    /// <summary>
    /// Returns the distance to the current target (if it exists).
    /// </summary>
    public float TargetDistance
    {
        get
        {
            if (Target != null)
            {
                return Vector3.Distance(transform.position, Target.transform.position);
            }
            return -1f;
        }
    }
    /// <summary>
    /// Returns true if the target is within the view frustrum of the camera.
    /// </summary>
    public bool TargetWithinViewAngle
    {
        get
        {
            if (Target != null && TargetDistance <= lineOfSightDistance)
            {
                Vector3 targetDir = Target.transform.position - transform.position;
                float targetAngle = Vector3.Angle(targetDir, transform.forward);

                if (targetAngle >= -lineOfSightAngle && targetAngle <= lineOfSightAngle)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Invoked when the player collides with the trigger.
    /// </summary>
    public event BreachTriggerDelegate BreachTriggerEvent = delegate { };

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        if (GameManager.Instance != null)
        {
            BreachTriggerEvent += GameManager.Instance.TriggerBreach;
            GameManager.Instance.BreachEvent += BreachResponse;
        }
        originalRotation = transform.eulerAngles;
        if (behaviour != 0)
        {
            StartRotation();
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (rotationTimer >= 0)
        {
            rotationTimer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationTimer / rotationTime);
            if (rotationTimer >= rotationTime)
            {
                rotationTimer = -1;
                idleTimer = 0;
            }
        }
        if (idleTimer >= 0)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                idleTimer = -1;
                StartRotation();
            }
        }
        if (TargetWithinViewAngle == true && Physics.Linecast(transform.position, Target.transform.position, out RaycastHit hit) == true && hit.collider.CompareTag("Player") == true)
        //if (Physics.SphereCast(transform.position, detectionRadius, transform.forward, out RaycastHit hit, viewDistance) == true && hit.collider.CompareTag("Player") == true
        //    || Physics.SphereCast(transform.position, detectionRadius, transform.forward + (Vector3.up.normalized * rayOffset), out hit, viewDistance) == true && hit.collider.CompareTag("Player") == true
        //    || Physics.SphereCast(transform.position, detectionRadius, transform.forward - (Vector3.up.normalized * rayOffset), out hit, viewDistance) == true && hit.collider.CompareTag("Player") == true)
        {

            if (awareTimer >= 0)
            {
                awareTimer += Time.deltaTime;
                fillImage.fillAmount = awareTimer / awareTime;
                if (awareTimer >= awareTime)
                {
                    awareTimer = -1;
                    fillImage.fillAmount = 0;

                    if (GameManager.Instance != null)
                    {
                        BreachTriggerEvent?.Invoke();
                    }
                    else if (hit.transform.TryGetComponent(out PlayerController controller) == true)
                    {
                        controller.TeleportToPosition(respawnPoint.position);
                    }
                }
            }
            else
            {
                awareTimer = 0;
            }
        }
        else if(awareTimer >= 0)
        {
            awareTimer -= Time.deltaTime;
            fillImage.fillAmount = awareTimer / awareTime;
            if (awareTimer <= 0)
            {
                awareTimer = -1;
            }
        }
    }

    /// <summary>
    /// Starts the rotation of the camera.
    /// </summary>
    private void StartRotation()
    {
        if(behaviour == 0 || behaviour == -1)
        {
            behaviour = 1;
        }
        else
        {
            behaviour = -1;
        }
        startRotation = transform.rotation;
        if(behaviour == 1)
        {
            targetRotation = Quaternion.Euler(new Vector3(originalRotation.x, originalRotation.y + rotationClamp, originalRotation.z));
        }
        else if (behaviour == -1)
        {
            targetRotation = Quaternion.Euler(new Vector3(originalRotation.x, originalRotation.y - rotationClamp, originalRotation.z));
        }
        rotationTimer = 0;
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Log("Responding to breach event.");
        if (GameManager.Instance != null)
        {
            BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
        }
    }

    /// <summary>
    /// Triggered when the player enter's the trigger.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true && Target != other.transform)
        {
            Target = other.transform;
        }
    }

    /// <summary>
    /// Triggered when the player leaves the trigger.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") == true && Target == other.transform)
        {
            Target = null;
        }
    }

    /// <summary>
    /// Draws gizmos in the scene view.
    /// </summary>
    /// <param name="other"></param>
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + ((transform.forward + (Vector3.up.normalized * rayOffset)) * viewDistance), detectionRadius);
        //Gizmos.DrawWireSphere(transform.position + ((transform.forward - (Vector3.up.normalized * rayOffset)) * viewDistance), detectionRadius);
        //Gizmos.DrawWireSphere(transform.position + (transform.forward * viewDistance), detectionRadius);
        //Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        //Gizmos.DrawRay(transform.position, (transform.forward + (Vector3.up.normalized * rayOffset)) * viewDistance);
        //Gizmos.DrawRay(transform.position, (transform.forward - (Vector3.up.normalized * rayOffset)) * viewDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lineOfSightDistance);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, lineOfSightAngle, 0) * transform.forward) * lineOfSightDistance);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, -lineOfSightAngle, 0) * transform.forward) * lineOfSightDistance);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(lineOfSightAngle, 0, 0) * transform.forward) * lineOfSightDistance);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(-lineOfSightAngle, 0, 0) * transform.forward) * lineOfSightDistance);
        if(TargetWithinViewAngle == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [PLAYER CONTROLLER] and the name of the associated game objected concatenated as a prefix.</param>
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
                    Debug.Log($"[SECURITY CAMERA] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[SECURITY CAMERA] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[SECURITY CAMERA] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}
