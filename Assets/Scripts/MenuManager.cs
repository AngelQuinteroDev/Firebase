using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;

    public void PlayGame()
    {
        string rawName = nameInput != null ? nameInput.text : string.Empty;
        string playerName = string.IsNullOrWhiteSpace(rawName) ? AppConstants.DefaultPlayerName : rawName.Trim();

        EnsurePlayerSession(playerName);
        AnalyticsManager.PendingPlayerName = playerName;
        
        PlayerPrefs.SetString(AppConstants.PlayerNameKey, playerName);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(AppConstants.SceneGame); 
    }

    private void EnsurePlayerSession(string playerName)
    {
        if (PlayerSession.Instance == null)
        {
            GameObject sessionObj = new GameObject("PlayerSession");
            PlayerSession session = sessionObj.AddComponent<PlayerSession>();
            session.PlayerName = playerName;
        }
        else
        {
            PlayerSession.Instance.PlayerName = playerName;
        }
    }
}
