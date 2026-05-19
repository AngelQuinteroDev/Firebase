using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Firestore;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    public static string PendingPlayerName = string.Empty;

    public AnalyticsData currentSession;
    private float sessionStartTime;
    private float idleTimeSeconds;

    private const int SaveTimeoutMilliseconds = 5000;

    [Header("Metrics")]
    [SerializeField] private float idealCompletionSeconds = 20f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentSession = null;
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.onGameStart.AddListener(OnGameStart);
        GameManager.Instance.onPlayerWin.AddListener(OnGameWin);
        GameManager.Instance.onPlayerLose.AddListener(OnGameLose);

        if (GameManager.Instance.IsPlaying && currentSession == null)
        {
            OnGameStart();
        }
    }

    public void OnGameStart()
    {
        string playerName = ResolvePlayerName();

        currentSession = new AnalyticsData
        {
            sessionId = Guid.NewGuid().ToString(),
            playerName = playerName,
            startTime = Timestamp.GetCurrentTimestamp(),
            wrongTurns = 0,
            collisions = 0,
            pauseCount = 0
        };

        sessionStartTime = Time.time;
        idleTimeSeconds = 0f;
        PendingPlayerName = string.Empty;
    }

    public void RegisterCollision()
    {
        if (currentSession != null) currentSession.collisions++;
    }

    public void RegisterWrongTurn()
    {
        if (currentSession != null) currentSession.wrongTurns++;
    }

    public void RegisterPause()
    {
        if (currentSession != null) currentSession.pauseCount++;
    }

    public void AddIdleTime(float time)
    {
        idleTimeSeconds += time;
    }

    public void OnGameWin()
    {
        EndSession(true);
    }

    public void OnGameLose()
    {
        EndSession(false);
    }

    private async void EndSession(bool won)
    {
        if (currentSession == null) return;

        ApplySessionMetrics(won);
        currentSession.finalScore = CalculateScore();

        await PersistSessionAsync();
        CacheLocalSummary();

        currentSession = null;

        UnityEngine.SceneManagement.SceneManager.LoadScene(AppConstants.SceneDashboard);
    }

    private void ApplySessionMetrics(bool won)
    {
        currentSession.reachedGoal = won;
        currentSession.duration = Time.time - sessionStartTime;
        currentSession.remainingTime = GameManager.Instance != null ? GameManager.Instance.TimeLeft : 0f;
        currentSession.averageDecisionTime = currentSession.pauseCount > 0 ? (idleTimeSeconds / currentSession.pauseCount) : 0f;
        currentSession.pathEfficiency = CalculatePathEfficiency();
    }

    private float CalculatePathEfficiency()
    {
        if (currentSession.duration <= 0f || idealCompletionSeconds <= 0f)
        {
            return 0f;
        }
        return Mathf.Clamp01(idealCompletionSeconds / currentSession.duration);
    }

    private async Task PersistSessionAsync()
    {
        try
        {
            if (FirestoreManager.Instance == null)
            {
                Debug.LogError("FirestoreManager instance missing.");
                return;
            }

            Task saveTask = FirestoreManager.Instance.SaveSessionAndHighscore(currentSession);
            Task timeoutTask = Task.Delay(SaveTimeoutMilliseconds);
            Task finished = await Task.WhenAny(saveTask, timeoutTask);

            if (finished == timeoutTask)
            {
                Debug.LogWarning("Firestore save timed out. Continuing to dashboard.");
                return;
            }

            await saveTask;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving to Firestore: " + ex.Message);
        }
    }

    private void CacheLocalSummary()
    {
        PlayerPrefs.SetInt(AppConstants.LastScoreKey, currentSession.finalScore);
        PlayerPrefs.SetString(AppConstants.LastStatusKey, currentSession.reachedGoal ? AppConstants.StatusWin : AppConstants.StatusLose);
    }

    private string ResolvePlayerName()
    {
        string name = PendingPlayerName;

        if (string.IsNullOrWhiteSpace(name))
        {
            if (PlayerSession.Instance != null && !string.IsNullOrWhiteSpace(PlayerSession.Instance.PlayerName))
            {
                name = PlayerSession.Instance.PlayerName;
            }
            else
            {
                name = PlayerPrefs.GetString(AppConstants.PlayerNameKey, AppConstants.DefaultPlayerName);
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return AppConstants.DefaultPlayerName;
        }

        return name.Trim();
    }

    private int CalculateScore()
    {
        int baseScore = currentSession.reachedGoal ? 10000 : 1000;
        float timePenalty = currentSession.duration * 10f;
        int collisionPenalty = currentSession.collisions * 50;
        int pausePenalty = currentSession.pauseCount * 20;
        int wrongTurnBonus = Mathf.Max(0, 1000 - (currentSession.wrongTurns * 10));

        int total = Mathf.RoundToInt(baseScore - timePenalty - collisionPenalty - pausePenalty + wrongTurnBonus);
        return Mathf.Max(0, total);
    }
}
