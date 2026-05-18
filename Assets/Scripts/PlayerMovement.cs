using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Analytics")]
    [SerializeField] private float directionChangeThreshold = 45f;
    [SerializeField] private float collisionCooldown = 0.25f;
    [SerializeField] private string[] collisionTags = new string[] { "Wall" };

    private bool wasMoving = false;
    private bool hasLastDecisionDir = false;
    private Vector3 lastDecisionDir = Vector3.forward;
    private float lastCollisionTime = -999f;

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

        Vector3 moveDir = new Vector3(input.x, 0f, input.y);

        // --- TRACKING DE ANALÍTICAS ---
        bool isMoving = moveDir.sqrMagnitude > 0.01f;
        
        if (AnalyticsManager.Instance != null && AnalyticsManager.Instance.currentSession != null)
        {
            if (isMoving)
            {
                float dist = moveDir.magnitude * moveSpeed * Time.deltaTime;
                AnalyticsManager.Instance.AddDistance(dist);

                Vector3 normalizedDir = moveDir.normalized;
                if (hasLastDecisionDir)
                {
                    if (Vector3.Angle(lastDecisionDir, normalizedDir) > directionChangeThreshold)
                    {
                        AnalyticsManager.Instance.AddDirectionChange();
                        lastDecisionDir = normalizedDir;
                    }
                }
                else
                {
                    lastDecisionDir = normalizedDir;
                    hasLastDecisionDir = true;
                }
            }
            else
            {
                AnalyticsManager.Instance.AddIdleTime(Time.deltaTime);
            }

            if (wasMoving && !isMoving)
            {
                AnalyticsManager.Instance.AddStop();
            }
        }
        
        wasMoving = isMoving;
        // ------------------------------

        if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;

        _cc.Move((moveDir * moveSpeed + Vector3.up * _velocity.y) * Time.deltaTime);

        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Mathf.Abs(hit.normal.y) < 0.1f && IsCollisionTag(hit.gameObject))
        {
            if (wasMoving && AnalyticsManager.Instance != null && _cc.velocity.magnitude > 0.1f)
            {
                RegisterWallCollision();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollisionTag(other.gameObject))
        {
            RegisterWallCollision();
        }
    }

    private void RegisterWallCollision()
    {
        if (Time.time - lastCollisionTime < collisionCooldown) return;
        lastCollisionTime = Time.time;
        AnalyticsManager.Instance?.AddWallCollision();
    }

    private bool IsCollisionTag(GameObject obj)
    {
        if (collisionTags == null || collisionTags.Length == 0)
        {
            return obj.tag == "Wall";
        }

        for (int i = 0; i < collisionTags.Length; i++)
        {
            string tagName = collisionTags[i];
            if (!string.IsNullOrEmpty(tagName) && obj.tag == tagName)
            {
                return true;
            }
        }
        return false;
    }
}
