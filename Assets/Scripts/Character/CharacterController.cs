using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody2D _rb;

    [Header("MOVEMENT SETTINGS:")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("JOYSTICK PACK:")]
    [SerializeField] private Joystick joystick; // Drag in your FixedJoystick, FloatingJoystick, etc.
    [SerializeField] private bool forceMobileInput = true;

    private Vector2 moveDirection;
    private Vector2 lastInputDirection = Vector2.down;
    private bool isMovementDisabled = false;

    // Dash fields
    private bool isDashing = false;
    private float dashTimeRemaining;
    private float dashSpeed;
    private Vector2 dashDirection;
    private bool isFrozen = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
                isDashing = false;
        }
    }

    private void FixedUpdate()
    {
        if (isMovementDisabled)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDashing)
        {
            _rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            Vector2 input = GetMoveInput();
            moveDirection = input.normalized;

            if (moveDirection != Vector2.zero)
                lastInputDirection = moveDirection;

            _rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private Vector2 GetMoveInput()
    {
        if (Application.isMobilePlatform || forceMobileInput)
            return joystick != null ? new Vector2(joystick.Horizontal, joystick.Vertical) : Vector2.zero;
        else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void TriggerDash(Vector2 direction, float speed, float duration)
    {
        if (isDashing) return;

        isDashing = true;
        dashDirection = direction.normalized;
        dashSpeed = speed;
        dashTimeRemaining = duration;
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(TemporarilyDisableMovement(duration));
    }

    private IEnumerator TemporarilyDisableMovement(float duration)
    {
        isMovementDisabled = true;
        _rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isMovementDisabled = false;
    }

    public void FreezePlayerFor(float duration)
    {
        if (!isFrozen)
            StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        Vector2 backup = MoveDirection;
        moveDirection = Vector2.zero;

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        moveDirection = backup;
    }

    // Public accessors
    public Vector2 MoveDirection => moveDirection;
    public Vector2 LastInputDirection => lastInputDirection;
    public bool IsDashing => isDashing;
    public void SetMovementEnabled(bool enabled) => isMovementDisabled = !enabled;
}
