using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento del jugador usando el NEW INPUT SYSTEM (Unity 6).
/// Optimizado para cámara cenital (top-down): W/S = eje Z, A/D = eje X.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed     = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -20f;

    private CharacterController _cc;
    private Vector3 _velocity;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        // ── Leer input ────────────────────────────────────────────────────────
        Vector2 input = Vector2.zero;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    input.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  input.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        }

        var gamepad = Gamepad.current;
        if (gamepad != null)
            input += gamepad.leftStick.ReadValue();

        input = Vector2.ClampMagnitude(input, 1f);

        // ── Dirección en ejes del mundo (cámara cenital) ──────────────────────
        // NO usamos la dirección de la cámara porque apunta recto hacia abajo
        // y al aplanar su forward a Y=0 queda un vector cero.
        // W/S controlan Z, A/D controlan X — ejes fijos del mundo.
        Vector3 moveDir = new Vector3(input.x, 0f, input.y);

        // ── Gravedad ─────────────────────────────────────────────────────────
        if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;

        // ── Aplicar movimiento ────────────────────────────────────────────────
        _cc.Move((moveDir * moveSpeed + Vector3.up * _velocity.y) * Time.deltaTime);

        // ── Rotar hacia la dirección de movimiento ────────────────────────────
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}