using UnityEngine;

public class FeedbackForm : MonoBehaviour
{
    private readonly string url = "https://docs.google.com/forms/d/e/1FAIpQLScbN20sRdRlNHhX8mS5KZUJeqJzCWk2n6N8_IkGJ9VS5qG3IQ/viewform?usp=sharing";

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }

    private void OnApplicationQuit()
    {
        Application.OpenURL(url);
    }
}
