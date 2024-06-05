using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float idleTime = 3f;
    [SerializeField] private float rotationTime = 5f;
    [SerializeField] private float rotationClamp = 30f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float detectionRadius = 1.5f;

    private float idleTimer = -1;
    private float rotationTimer = -1;
    private int behaviour = 0;
    private Vector3 originalRotation;
    private Quaternion startRotation;
    private Quaternion targetRotation;

    /// <summary>
    /// Invoked when the player collides with the trigger.
    /// </summary>
    public event BreachTriggerDelegate BreachTriggerEvent = delegate { };

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        BreachTriggerEvent += GameManager.Instance.TriggerBreach;
        GameManager.Instance.BreachEvent += BreachResponse;
        originalRotation = transform.eulerAngles;
        StartRotation();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if(rotationTimer >= 0)
        {
            rotationTimer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationTimer / rotationTime);
            if(rotationTimer >= rotationTime)
            {
                rotationTimer = -1;
                idleTimer = 0;
            }
        }
        if(idleTimer >= 0)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer >= idleTime)
            {
                idleTimer = -1;
                StartRotation();
            }
        }
        if(Physics.SphereCast(transform.position, detectionRadius, transform.forward, out RaycastHit hit, viewDistance) == true && hit.collider.CompareTag("Player") == true)
        {
            BreachTriggerEvent?.Invoke();
        }
    }

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
        Debug.Log("Security camera responding to breach event.");
        BreachTriggerEvent -= GameManager.Instance.TriggerBreach;
    }
}
