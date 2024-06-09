using UnityEngine;

public class FeedbackForm : MonoBehaviour
{
    private readonly string url = "https://forms.gle/z7omXdrxX3xG645v6";

    private void OnApplicationQuit()
    {
        Application.OpenURL(url);
    }
}
