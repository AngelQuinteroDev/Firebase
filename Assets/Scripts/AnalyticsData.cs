[System.Serializable]
public class AnalyticsData
{
    public string playerName;

    public float durationSeconds;

    public int finalScore;

    public bool won;

    public int stepsTaken;

    public int wrongTurns;

    public int deadEndsVisited;

    public int timesHitWall;

    public float averageDecisionTime;

    public int backtrackingCount;

    public float explorationPercentage;

    public int pauseCount;

    public long startTimestamp;
}