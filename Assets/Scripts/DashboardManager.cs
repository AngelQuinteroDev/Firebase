using UnityEngine;
using TMPro;

public class DashboardManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text rankText;

    private async void Start()
    {
        // Cursor visible por si acaso
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Recuperar datos de PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName", "Anonimo");
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        string status = PlayerPrefs.GetString("LastStatus", "");

        nameText.text = $"Jugador: {playerName}";
        statusText.text = $"Estado de última partida: {status}";
        scoreText.text = $"Puntaje: {lastScore}";
        rankText.text = "Calculando ranking temporal...";

        // Obtener el ranking global en Highscores desde Firestore
        if (FirestoreManager.Instance != null)
        {
            int rank = await FirestoreManager.Instance.GetPlayerRank(playerName, lastScore);
            if (rank > 0)
            {
                rankText.text = $"Ranking global: #{rank}";
            }
            else
            {
                rankText.text = "No se pudo calcular el ranking";
            }
        }
        else
        {
            rankText.text = "Servidor desconectado";
        }
    }

    public void ViewWebDashboard()
    {
        // Reemplazar con el link de tu Dashboard Web real
        Application.OpenURL("https://tu-dashboard-web.com");
    }

    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}