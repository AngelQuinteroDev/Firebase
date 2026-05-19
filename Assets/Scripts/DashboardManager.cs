using UnityEngine;
using TMPro;

public class DashboardManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text rankText;

    private async void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string playerName = PlayerPrefs.GetString(AppConstants.PlayerNameKey, AppConstants.DefaultPlayerName);
        int lastScore = PlayerPrefs.GetInt(AppConstants.LastScoreKey, 0);
        string status = PlayerPrefs.GetString(AppConstants.LastStatusKey, "");

        nameText.text = $"Player: {playerName}";
        statusText.text = $"Last result: {status}";
        scoreText.text = $"Score: {lastScore}";
        rankText.text = "Calculating rank...";

        if (FirestoreManager.Instance != null)
        {
            int rank = await FirestoreManager.Instance.GetPlayerRank(playerName, lastScore);
            if (rank > 0)
            {
                rankText.text = $"Global rank: #{rank}";
            }
            else
            {
                rankText.text = "Rank unavailable";
            }
        }
        else
        {
            rankText.text = "Server not ready";
        }
    }

    public void ViewWebDashboard()
    {
        Application.OpenURL("https://dashboard-web-flax-pi.vercel.app/");
    }

    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(AppConstants.SceneGame);
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(AppConstants.SceneMenu);
    }
}