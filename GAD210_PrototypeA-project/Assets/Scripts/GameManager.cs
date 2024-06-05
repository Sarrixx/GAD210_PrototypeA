using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

#region AUTHOR & COPYRIGHT DETAILS
/// Original Author: Joshua Ferguson
/// Contact: Joshua Ferguson <Josh.Ferguson@smtafe.wa.edu.au>.
/// Last Updated: March, 2024
#endregion

public delegate void BreachTriggerDelegate();

/// <summary>
/// Script responsible for managing gameplay loop.
/// </summary>
public class GameManager : MonoBehaviour, ILoggable
{
    public delegate void BreachEventDelegate();

    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("The amount of time the player has to escape when a breach is triggered.")]
    [SerializeField] private float timeToEscape = 120f;
    [SerializeField] private AudioSource alarmSource;
    [SerializeField] private GameObject[] standardLighting;
    [SerializeField] private GameObject[] alarmLighting;

    private float escapeTimer = -1;

    /// <summary>
    /// Returns true if a breach has been triggered.
    /// </summary>
    public bool BreachTriggered { get; private set; } = false;

    /// <summary>
    /// Invoked when a breach is triggered.
    /// </summary>
    public event BreachEventDelegate BreachEvent = delegate { };

    /// <summary>
    /// Singleton reference to class.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (standardLighting.Length > 0 && alarmLighting.Length > 0)
        {
            foreach (GameObject lightingObject in standardLighting)
            {
                if (lightingObject.activeSelf != true)
                {
                    lightingObject.SetActive(true);
                }
            }
            foreach (GameObject lightingObject in alarmLighting)
            {
                if (lightingObject.activeSelf != false)
                {
                    lightingObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        BreachEvent += BreachResponse;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }

        if(escapeTimer >= 0)
        {
            escapeTimer += Time.deltaTime;
            if(escapeTimer >= timeToEscape)
            {
                escapeTimer = -1;
                //game over
                Debug.Log("Game over.");
            }
        }
    }

    /// <summary>
    /// Functionality executed in response to a breach event triggering.
    /// </summary>
    private void BreachResponse()
    {
        Debug.Log("Game manager responding to breach trigger.");
        escapeTimer = 0;
        if(standardLighting.Length > 0 && alarmLighting.Length > 0)
        {
            foreach(GameObject lightingObject in standardLighting)
            {
                lightingObject.SetActive(false);
            }
            foreach(GameObject lightingObject in alarmLighting)
            {
                lightingObject.SetActive(true);
            }
        }
        if (alarmSource != null && alarmSource.clip != null)
        {
            alarmSource.loop = true;
            alarmSource.Play();
        }
    }

    /// <summary>
    /// Invokes the breach event if it hasn't already been triggered.
    /// </summary>
    public void TriggerBreach()
    {
        if(BreachTriggered == false)
        {
            Debug.Log("Breach triggered.");
            BreachEvent?.Invoke();
            BreachTriggered = true;
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

/// <summary>
/// Interface that defines the framework for tracking logged messages.
/// </summary>
public interface ILoggable
{
    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.</param>
    /// <param name="level">A level of 0 prints a standard message. A level of 1 prints a warning message. A level of 2 prints an error message.</param>
    public void Log(string message, int level = 0);
}

/// <summary>
/// Interface that defines the framework for breach triggers.
/// </summary>
public interface IBreachTrigger
{
    event BreachTriggerDelegate BreachTriggerEvent;
}