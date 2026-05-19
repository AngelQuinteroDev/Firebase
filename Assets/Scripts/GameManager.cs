using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Timer")]
    [SerializeField] private float timeLimit = 60f;

    [Header("Events")]
    public UnityEvent onGameStart;
    public UnityEvent onPlayerWin;
    public UnityEvent onPlayerLose;
    public UnityEvent<float> onTimerUpdate;

    public bool IsPlaying { get; private set; }
    public float TimeLeft { get; private set; }
    public bool GameFinished { get; private set; }
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

    public void StartGame()
    {
        TimeLeft     = timeLimit;
        IsPlaying    = true;
        GameFinished = false;
        onGameStart?.Invoke();
    }

    public void TriggerWin()
    {
        if (GameFinished) return;
        GameFinished = true;
        IsPlaying    = false;
        Debug.Log("[GameManager] Victory");
        onPlayerWin?.Invoke();
    }

    public void TriggerLose()
    {
        if (GameFinished) return;
        GameFinished = true;
        IsPlaying    = false;
        Debug.Log("[GameManager] Time up - lose");
        onPlayerLose?.Invoke();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
