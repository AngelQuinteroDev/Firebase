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
    private float totalDistanceTraveled;
    private const int SaveTimeoutMs = 5000;
    [Header("Metricas")]
    [SerializeField] private float idealPathLength = 50f;

    [Header("Configuraci�n Sesi�n")]
    public string currentPlayerName = "Jugador_Anonimo";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentSession = null;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameStart.AddListener(OnGameStart);
            GameManager.Instance.onPlayerWin.AddListener(OnGameWin);
            GameManager.Instance.onPlayerLose.AddListener(OnGameLose);

            if (GameManager.Instance.IsPlaying && currentSession == null)
            {
                OnGameStart();
            }
        }
    }

    public void OnGameStart()
    {
        string savedName = PendingPlayerName;
        if (string.IsNullOrWhiteSpace(savedName))
        {
            if (PlayerSession.Instance != null && !string.IsNullOrWhiteSpace(PlayerSession.Instance.PlayerName))
            {
                savedName = PlayerSession.Instance.PlayerName;
            }
            else
            {
                savedName = PlayerPrefs.GetString("PlayerName", currentPlayerName);
            }
        }

        if (string.IsNullOrWhiteSpace(savedName))
        {
            savedName = "Jugador_Anonimo";
        }
        else
        {
            savedName = savedName.Trim();
        }


        currentSession = new AnalyticsData
        {
            sessionId = Guid.NewGuid().ToString(), // ID �nico
            playerName = savedName,
            startTime = Timestamp.GetCurrentTimestamp(),
            wrongTurns = 0,
            collisions = 0,
            pauseCount = 0
        };
        sessionStartTime = Time.time;
        idleTimeSeconds = 0f;
        totalDistanceTraveled = 0f;
    }

    public void AddWallCollision() { if(currentSession != null) currentSession.collisions++; }
    public void AddDirectionChange() { if(currentSession != null) currentSession.wrongTurns++; }
    public void AddDistance(float dist) { totalDistanceTraveled += dist; }
    public void AddStop() { if(currentSession != null) currentSession.pauseCount++; }
    public void AddIdleTime(float time) { idleTimeSeconds += time; }

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

        currentSession.reachedGoal = won;
        currentSession.duration = Time.time - sessionStartTime;
        currentSession.remainingTime = (GameManager.Instance != null) ? GameManager.Instance.TimeLeft : 0f;

        // averageDecisionTime
        currentSession.averageDecisionTime = (currentSession.pauseCount > 0) ? (idleTimeSeconds / currentSession.pauseCount) : 0f;

        // Path efficiency: ideal length / traveled length
        if (totalDistanceTraveled > 0f && idealPathLength > 0f)
        {
            currentSession.pathEfficiency = Mathf.Clamp01(idealPathLength / totalDistanceTraveled);
        }
        else
        {
            currentSession.pathEfficiency = 0f;
        }

        currentSession.finalScore = CalculateScore();

        try
        {
            if (FirestoreManager.Instance != null)
            {
                Task saveTask = FirestoreManager.Instance.SaveSessionAndHighscore(currentSession);
                Task timeoutTask = Task.Delay(SaveTimeoutMs);
                Task finished = await Task.WhenAny(saveTask, timeoutTask);

                if (finished == timeoutTask)
                {
                    Debug.LogWarning("Firestore save timed out. Continuing to dashboard.");
                }
                else
                {
                    await saveTask;
                }
            }
            else
            {
                Debug.LogError("FirestoreManager instance missing.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error guardando en Firestore: " + ex.Message);
        }

        PlayerPrefs.SetInt("LastScore", currentSession.finalScore);
        PlayerPrefs.SetString("LastStatus", currentSession.reachedGoal ? "Victoria" : "Derrota");

        currentSession = null; 

        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }

    private int CalculateScore()
    {
        int baseScore = currentSession.reachedGoal ? 10000 : 1000;
        float timePenalty = currentSession.duration * 10f;
        int wallPenalty = currentSession.collisions * 50;
        int stopPenalty = currentSession.pauseCount * 20;
        int directionBonus = Mathf.Max(0, 1000 - (currentSession.wrongTurns * 10));

        int total = Mathf.RoundToInt(baseScore - timePenalty - wallPenalty - stopPenalty + directionBonus);
        return Mathf.Max(0, total);
    }
}
