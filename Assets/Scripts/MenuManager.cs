using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;

    public void PlayGame()
    {
        string rawName = nameInput != null ? nameInput.text : string.Empty;
        string playerName = string.IsNullOrWhiteSpace(rawName) ? "Jugador_Anonimo" : rawName.Trim();

        EnsurePlayerSession(playerName);
        AnalyticsManager.PendingPlayerName = playerName;
        
        // Guardar nombre transversalmente en PlayerPrefs
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        
        // Cargar escena de juego
        SceneManager.LoadScene("Game"); 
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
