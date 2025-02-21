using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        // If running in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running in a build, quit the application
            Application.Quit();
#endif
    }
}
