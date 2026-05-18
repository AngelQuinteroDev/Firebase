using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton que gestiona el estado global del minijuego:
/// temporizador, victoria y derrota.
/// Coloca este script en un GameObject vacío llamado "GameManager".
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Inspector ─────────────────────────────────────────────────────────────
    [Header("Tiempo")]
    [SerializeField] private float timeLimit = 60f;   // segundos por nivel

    [Header("Eventos (arrastra objetos de UI aquí)")]
    public UnityEvent onGameStart;
    public UnityEvent onPlayerWin;
    public UnityEvent onPlayerLose;
    public UnityEvent<float> onTimerUpdate;   // envía tiempo restante

    // ── Estado ────────────────────────────────────────────────────────────────
    public bool  IsPlaying    { get; private set; }
    public float TimeLeft     { get; private set; }
    public bool  GameFinished { get; private set; }

    // ── Unity ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        TimeLeft -= Time.deltaTime;
        onTimerUpdate?.Invoke(TimeLeft);

        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            TriggerLose();
        }
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Inicia o reinicia el juego.</summary>
    public void StartGame()
    {
        TimeLeft     = timeLimit;
        IsPlaying    = true;
        GameFinished = false;
        onGameStart?.Invoke();
    }

    /// <summary>Llamado por ExitTrigger cuando el jugador llega a la meta.</summary>
    public void TriggerWin()
    {
        if (GameFinished) return;
        GameFinished = true;
        IsPlaying    = false;
        Debug.Log("[GameManager] ¡Victoria!");
        onPlayerWin?.Invoke();
    }

    /// <summary>Llamado internamente cuando el tiempo se agota.</summary>
    public void TriggerLose()
    {
        if (GameFinished) return;
        GameFinished = true;
        IsPlaying    = false;
        Debug.Log("[GameManager] Tiempo agotado — derrota.");
        onPlayerLose?.Invoke();
    }

    /// <summary>Reinicia la escena actual.</summary>
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
