using UnityEngine;

#region AUTHOR & COPYRIGHT DETAILS
/// Original Author: Joshua Ferguson
/// Contact: Joshua Ferguson <Josh.Ferguson@smtafe.wa.edu.au>.
/// Last Updated: March, 2024
#endregion

/// <summary>
/// Script responsible for playing footsteps when the player moves.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class Footsteps : MonoBehaviour, ILoggable
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("This only exists in the complete project version of the script (not in the student version of this script)." +
        "Must be set to true when using the 'complete' version of the PlayerController component.")]
    [SerializeField] private bool checkForCompleteController = false;
    [Tooltip("The number of seconds between each footstep sound.")]
    [SerializeField][Range(0.1f, 2f)] private float timeBetweenSteps = 0.5f;
    [Tooltip("A list to the foostep audio clips. (The list will be randomly selected from each time a footstep is triggered.)")]
    [SerializeField] private AudioClip[] footstepClips;

    private float timer = 0f;
    private AudioSource aSrc;
    private CharacterController controller;

    /// <summary>
    /// Adjusts the speed of the time between each footstep.
    /// </summary>
    public float TimeBetweenStepsMultiplier { get; set; } = 1f;

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        if (TryGetComponent(out controller) == false)
        {
            Log($"{gameObject.name} requires a Character Controller component!", 1);
        }
        if (TryGetComponent(out aSrc) == false)
        {
            Log($"{gameObject.name} requires an Audio Source component.", 1);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if(aSrc != null)
        {
            aSrc.loop = false;
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (controller != null && controller.isGrounded == true)
        {
            if((checkForCompleteController == true && PlayerController.Instance.MovementEnabled == true) || (checkForCompleteController == false && PlayerController.Instance.MovementEnabled == true))
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    if (timer >= 0f)
                    {
                        timer += Time.deltaTime;
                        if (timer >= timeBetweenSteps * TimeBetweenStepsMultiplier)
                        {
                            if (aSrc != null && footstepClips.Length > 0)
                            {
                                aSrc.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                            }
                            timer = 0f;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [GAME MANAGER] and the name of the associated game objected concatenated as a prefix.</param>
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
                    Debug.Log($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}
