using Firebase.Firestore;

[System.Serializable]
[FirestoreData]
public class AnalyticsData
{
    [FirestoreProperty] public string sessionId { get; set; }
    [FirestoreProperty] public string playerName { get; set; }
    [FirestoreProperty] public Timestamp startTime { get; set; }
    [FirestoreProperty] public float duration { get; set; }
    [FirestoreProperty] public int finalScore { get; set; }
    [FirestoreProperty] public bool reachedGoal { get; set; }
    [FirestoreProperty] public float remainingTime { get; set; }
    [FirestoreProperty] public int wrongTurns { get; set; }
    [FirestoreProperty] public int collisions { get; set; }
    [FirestoreProperty] public float pathEfficiency { get; set; }
    [FirestoreProperty] public int pauseCount { get; set; }
    [FirestoreProperty] public float averageDecisionTime { get; set; }
}
