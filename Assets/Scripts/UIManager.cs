using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text messageText;

    [Header("Paneles de resultado")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Color del timer")]
    [SerializeField] private Color normalColor  = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor  = Color.red;
    [SerializeField] private float warningThreshold = 20f;
    [SerializeField] private float dangerThreshold  = 10f;

    private void Start()
    {
        if (winPanel  != null) winPanel .SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (messageText != null) messageText.text = "¡Llega a la salida!";
    }

    // ── Llamado por GameManager.onTimerUpdate (UnityEvent<float>) ─────────────
    public void UpdateTimer(float timeLeft)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Color progresivo
        if      (timeLeft <= dangerThreshold)  timerText.color = dangerColor;
        else if (timeLeft <= warningThreshold) timerText.color = warningColor;
        else                                   timerText.color = normalColor;
    }

    // ── Llamado por GameManager.onPlayerWin ───────────────────────────────────
    public void ShowWin()
    {
        // En lugar de mostrar el panel con botones, damos feedback de carga
        if (winPanel != null) winPanel.SetActive(false);
        if (messageText != null) messageText.text = "¡Meta alcanzada!\n\nCalculando puntuación...";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ShowLose()
    {
        // En lugar de mostrar el panel con botones, damos feedback de carga
        if (losePanel != null) losePanel.SetActive(false);
        if (messageText != null) messageText.text = "Tiempo agotado...\n\nCalculando puntuación...";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    // ── Botón Reiniciar (asigna en el Inspector al onClick del Button) ─────────
    public void OnRestartButton()
    {
        GameManager.Instance?.RestartGame();
    }
}
