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
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LoadNextLevel();
        }
        else
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }
} 