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
        DontDestroyOnLoad(gameObject); // Persiste a través de las escenas
    }

    async void Start()
    {
        // Verificar dependencias Firebase
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

        // Ensure playerName is usable for document IDs
        string rawPlayerName = sessionData.playerName;
        if (string.IsNullOrWhiteSpace(rawPlayerName))
        {
            rawPlayerName = "Jugador_Anonimo";
        }
        rawPlayerName = rawPlayerName.Trim();
        if (string.IsNullOrWhiteSpace(rawPlayerName))
        {
            rawPlayerName = "Jugador_Anonimo";
        }
        sessionData.playerName = rawPlayerName;
        string safePlayerDocId = rawPlayerName.Replace("/", "_");

        if (!isReady || db == null)
        {
            Debug.LogError("Firestore no está listo para enviar datos.");
            return;
        }

        try
        {
            // 1. Guardar la sesión con ID único automático
            DocumentReference sessionRef = db.Collection("sessions").Document(); // Document() sin params genera un ID único
            sessionData.sessionId = sessionRef.Id;
            await sessionRef.SetAsync(sessionData);
            Debug.Log($"Sesión guardada en Firestore con ID: {sessionRef.Id}");

            // 2. Guardar/Actualizar el Highscore
            // highscores/{playerName}
            DocumentReference highscoreRef = db.Collection("highscores").Document(safePlayerDocId);
            DocumentSnapshot snapshot = await highscoreRef.GetSnapshotAsync();

            int currentHighscore = 0;
            if (snapshot.Exists && snapshot.TryGetValue("score", out int existingScore))
            {
                currentHighscore = existingScore;
            }

            // Si el nuevo score es mayor, o si no existía, lo actualizamos
            if (sessionData.finalScore > currentHighscore)
            {
                Dictionary<string, object> highscoreDict = new Dictionary<string, object>
                {
                    { "playerName", sessionData.playerName },
                    { "score", sessionData.finalScore }
                };
                
                // Merge para actualizar o crear sin borrar otros campos accidentales si hubieran futuros
                await highscoreRef.SetAsync(highscoreDict, SetOptions.MergeAll);
                Debug.Log($"Highscore actualizado para {sessionData.playerName}: {sessionData.finalScore}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error guardando datos en Firestore: {ex.Message}");
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
            Debug.LogError("Error obteniendo ranking: " + ex.Message);
            return -1;
        }
    }
}
