using UnityEngine;
public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    public AnalyticsData data;

    private void Awake()
    {
        Instance = this;
    }
}
