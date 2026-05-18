using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona toda la UI del minijuego.
/// Requiere TextMeshPro (ya incluido en Unity 6).
///
/// Jerarquía de Canvas sugerida:
///   Canvas
///   ├── HUD (Panel)
///   │   ├── TimerText   (TMP)   → ej. "00:45"
///   │   └── MessageText (TMP)   → ej. "¡Llega a la salida!"
///   ├── WinPanel  (Panel)  → desactivado al inicio
///   │   ├── TitleText  (TMP)
///   │   └── RestartButton
///   └── LosePanel (Panel) → desactivado al inicio
///       ├── TitleText  (TMP)
///       └── RestartButton
/// </summary>
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
        if (winPanel  != null) winPanel .SetActive(true);
        if (messageText != null) messageText.text = "¡Meta alcanzada!";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    // ── Llamado por GameManager.onPlayerLose ──────────────────────────────────
    public void ShowLose()
    {
        if (losePanel != null) losePanel.SetActive(true);
        if (messageText != null) messageText.text = "Tiempo agotado...";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    // ── Botón Reiniciar (asigna en el Inspector al onClick del Button) ─────────
    public void OnRestartButton()
    {
        GameManager.Instance?.RestartGame();
    }
}
