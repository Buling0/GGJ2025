using UnityEngine;
using UnityEngine.UI;

public class LevelOkManager : MonoBehaviour
{
    public Button okButton;

    private void Start()
    {
        if (okButton != null)
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
        }
        else
        {
            Debug.LogError("OK Button is not assigned in the inspector.");
        }
    }

    public void OnOkButtonClicked()
    {
        Debug.Log("OK Button clicked.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("GameManager instance not found.");
        }
    }
} 