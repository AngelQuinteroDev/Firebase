using UnityEngine;

/// <summary>
/// Coloca este script en un GameObject vacío con un Collider marcado como "Is Trigger"
/// justo en la salida del laberinto. Cuando el jugador entre, notifica al GameManager.
///
/// Pasos rápidos:
///  1. Crea un GameObject vacío → llámalo "ExitZone".
///  2. Añade un Box Collider → activa "Is Trigger".
///  3. Ajusta el tamaño para cubrir la salida.
///  4. Añade este script al mismo GameObject.
///  5. Asegúrate de que el jugador tenga el tag "Player".
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Efecto visual opcional")]
    [SerializeField] private GameObject exitVFX;   // partícula o luz (opcional)

    private void Start()
    {
        if (exitVFX != null) exitVFX.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (exitVFX != null) exitVFX.SetActive(false);
        GameManager.Instance?.TriggerWin();
    }
}
