using Firebase;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance;
    private FirebaseFirestore db;
    private bool isReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            Debug.Log("Firebase Ready");
            db = FirebaseFirestore.DefaultInstance;
            isReady = true;
        }
        else
        {
            Debug.LogError("Firebase error: " + status);
        }
    }

    public async Task SaveSessionAndHighscore(AnalyticsData sessionData)
    {
        if (sessionData == null)
        {
            Debug.LogError("sessionData is null.");
            return;
        }

        string rawPlayerName = sessionData.playerName;
        if (string.IsNullOrWhiteSpace(rawPlayerName))
        {
            rawPlayerName = AppConstants.DefaultPlayerName;
        }
        rawPlayerName = rawPlayerName.Trim();
        if (string.IsNullOrWhiteSpace(rawPlayerName))
        {
            rawPlayerName = AppConstants.DefaultPlayerName;
        }
        sessionData.playerName = rawPlayerName;
        string safePlayerDocId = rawPlayerName.Replace("/", "_");

        if (!isReady || db == null)
        {
            Debug.LogError("Firestore is not ready.");
            return;
        }

        try
        {
            DocumentReference sessionRef = db.Collection("sessions").Document();
            sessionData.sessionId = sessionRef.Id;
            await sessionRef.SetAsync(sessionData);
            Debug.Log($"Session saved with ID: {sessionRef.Id}");

            DocumentReference highscoreRef = db.Collection("highscores").Document(safePlayerDocId);
            DocumentSnapshot snapshot = await highscoreRef.GetSnapshotAsync();

            int currentHighscore = 0;
            if (snapshot.Exists && snapshot.TryGetValue("score", out int existingScore))
            {
                currentHighscore = existingScore;
            }

            if (sessionData.finalScore > currentHighscore)
            {
                Dictionary<string, object> highscoreDict = new Dictionary<string, object>
                {
                    { "playerName", sessionData.playerName },
                    { "score", sessionData.finalScore }
                };

                await highscoreRef.SetAsync(highscoreDict, SetOptions.MergeAll);
                Debug.Log($"Highscore updated for {sessionData.playerName}: {sessionData.finalScore}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving to Firestore: {ex.Message}");
        }
    }

    public async Task<int> GetPlayerRank(string playerName, int playerScore)
    {
        if (!isReady || db == null) return -1;
        try
        {
            Query query = db.Collection("highscores").WhereGreaterThan("score", playerScore);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Count() + 1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error getting rank: " + ex.Message);
            return -1;
        }
    }
}
