using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    public const string PLAYERNAMEKEY = "PlayerName";
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    private int minNameLength = 3;
    private int maxNameLength = 15;


    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        nameField.text = PlayerPrefs.GetString(PLAYERNAMEKEY, string.Empty);
        HandleNameChanged();

    }

    public void HandleNameChanged()
    {
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
    }

    public void Connect() 
    {
        PlayerPrefs.SetString(PLAYERNAMEKEY, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
