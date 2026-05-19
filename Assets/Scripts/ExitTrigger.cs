using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Optional VFX")]
    [SerializeField] private GameObject exitVFX;

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
