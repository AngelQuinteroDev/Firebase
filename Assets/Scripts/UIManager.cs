using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text messageText;

    [Header("Result Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Timer Colors")]
    [SerializeField] private Color normalColor  = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor  = Color.red;
    [SerializeField] private float warningThreshold = 20f;
    [SerializeField] private float dangerThreshold  = 10f;

    private void Start()
    {
        if (winPanel  != null) winPanel .SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (messageText != null) messageText.text = "Reach the exit!";
    }

    public void UpdateTimer(float timeLeft)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if      (timeLeft <= dangerThreshold)  timerText.color = dangerColor;
        else if (timeLeft <= warningThreshold) timerText.color = warningColor;
        else                                   timerText.color = normalColor;
    }

    public void ShowWin()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (messageText != null) messageText.text = "Goal reached!\n\nCalculating score...";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ShowLose()
    {
        if (losePanel != null) losePanel.SetActive(false);
        if (messageText != null) messageText.text = "Time up...\n\nCalculating score...";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void OnRestartButton()
    {
        GameManager.Instance?.RestartGame();
    }
}
