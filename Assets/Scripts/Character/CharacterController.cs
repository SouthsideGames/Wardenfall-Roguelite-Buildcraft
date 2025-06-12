using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour, IStats
{
    private Rigidbody2D _rb;
    private bool isMovementDisabled = false;

    [Header("SETTINGS:")]
    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;

    [Tooltip("Current movement direction.")]
    private Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;

    private Vector2 lastInputDirection = Vector2.down;
    public Vector2 LastInputDirection => lastInputDirection;

    // Dash State
    private bool isDashing = false;
    private float dashTimer;
    private Vector2 dashVelocity;

    void Start() => _rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (!GameManager.Instance.InGameState() || isMovementDisabled)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        moveDirection = InputManager.Instance.GetMoveVector();
        if (moveDirection.magnitude > 0.1f) // Add dead zone
        {
            moveDirection = moveDirection.normalized;
            lastInputDirection = moveDirection;
        }
        else
        {
            moveDirection = Vector2.zero;
        }

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            _rb.linearVelocity = dashVelocity;

            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            _rb.linearVelocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
        }
    }


    public void TriggerDash(Vector2 direction, float dashSpeed, float duration)
    {
        isDashing = true;
        dashTimer = duration;
        dashVelocity = direction.normalized * dashSpeed;
    }

    public void UpdateWeaponStats(CharacterStats _statsManager)
    {
        float moveSpeedPercent = _statsManager.GetStatValue(Stat.MoveSpeed) / 100;
        moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    }

    public void DisableMovement(float duration)
    {
        isMovementDisabled = true;
        _rb.linearVelocity = Vector2.zero;
        Invoke(nameof(EnableMovement), duration);
    }

    public void EnableMovement() => isMovementDisabled = false;

    public void SetMovementMultiplier(float multiplier) => moveSpeed = baseMoveSpeed * multiplier;

    private void OnDisable() => CancelInvoke(nameof(EnableMovement));
}
