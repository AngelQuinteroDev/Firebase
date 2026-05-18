using UnityEngine;

/// <summary>
/// Cámara que sigue al jugador suavemente.
/// Soporta dos modos: Third Person y Top-Down (cenital).
///
/// Uso:
///  1. Adjunta este script a tu Main Camera.
///  2. Asigna el Transform del jugador en el Inspector.
///  3. Elige el modo en el enum CameraMode.
/// </summary>
public class MazeCamera : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, TopDown }

    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Modo")]
    [SerializeField] private CameraMode mode = CameraMode.ThirdPerson;

    [Header("Third Person")]
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 4f, -6f);
    [SerializeField] private float   thirdPersonSmooth = 8f;

    [Header("Top Down")]
    [SerializeField] private float topDownHeight = 14f;
    [SerializeField] private float topDownSmooth  = 6f;

    private void LateUpdate()
    {
        if (target == null) return;

        switch (mode)
        {
            case CameraMode.ThirdPerson:
                FollowThirdPerson();
                break;
            case CameraMode.TopDown:
                FollowTopDown();
                break;
        }
    }

    private void FollowThirdPerson()
    {
        Vector3 desiredPos = target.position
                           + target.rotation * thirdPersonOffset;

        transform.position = Vector3.Lerp(
            transform.position, desiredPos, thirdPersonSmooth * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1f);
    }

    private void FollowTopDown()
    {
        Vector3 desiredPos = new Vector3(
            target.position.x,
            target.position.y + topDownHeight,
            target.position.z);

        transform.position = Vector3.Lerp(
            transform.position, desiredPos, topDownSmooth * Time.deltaTime);

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
