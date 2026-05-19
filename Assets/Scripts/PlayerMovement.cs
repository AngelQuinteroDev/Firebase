using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;
    private Vector3 velocity;

    [Header("Analytics")]
    [SerializeField] private float wrongTurnAngleThreshold = 45f;
    [SerializeField] private float collisionCooldownSeconds = 0.25f;
    [SerializeField] private string[] collisionTags = new string[] { "Wall" };

    private bool wasMoving = false;
    private bool hasLastDecisionDirection = false;
    private Vector3 lastDecisionDirection = Vector3.forward;
    private float lastCollisionTime = -999f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;
        UpdateMovement();
    }

    private void UpdateMovement()
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

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        
        if (AnalyticsManager.Instance != null && AnalyticsManager.Instance.currentSession != null)
        {
            if (isMoving)
            {
                Vector3 normalizedDirection = moveDirection.normalized;
                if (hasLastDecisionDirection)
                {
                    if (Vector3.Angle(lastDecisionDirection, normalizedDirection) > wrongTurnAngleThreshold)
                    {
                        AnalyticsManager.Instance.RegisterWrongTurn();
                        lastDecisionDirection = normalizedDirection;
                    }
                }
                else
                {
                    lastDecisionDirection = normalizedDirection;
                    hasLastDecisionDirection = true;
                }
            }
            else
            {
                AnalyticsManager.Instance.AddIdleTime(Time.deltaTime);
            }

            if (wasMoving && !isMoving)
            {
                AnalyticsManager.Instance.RegisterPause();
            }
        }
        
        wasMoving = isMoving;

        if (characterController.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        characterController.Move((moveDirection * moveSpeed + Vector3.up * velocity.y) * Time.deltaTime);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Mathf.Abs(hit.normal.y) < 0.1f && IsCollisionTag(hit.gameObject))
        {
            if (wasMoving && AnalyticsManager.Instance != null && characterController.velocity.magnitude > 0.1f)
            {
                RegisterCollision();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollisionTag(other.gameObject))
        {
            RegisterCollision();
        }
    }

    private void RegisterCollision()
    {
        if (Time.time - lastCollisionTime < collisionCooldownSeconds) return;
        lastCollisionTime = Time.time;
        AnalyticsManager.Instance?.RegisterCollision();
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
