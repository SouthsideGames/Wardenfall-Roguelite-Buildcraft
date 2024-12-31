using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour, IStats
{
    private Rigidbody2D _rb;

    [Header("SETTINGS:")]
    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;

    [Tooltip("Current movement direction.")]
    private Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;


    void Start() => _rb = GetComponent<Rigidbody2D>(); 

    private void FixedUpdate()
    {
        if (!GameManager.Instance.InGameState())
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        moveDirection = InputManager.Instance.GetMoveVector();
        _rb.linearVelocity = InputManager.Instance.GetMoveVector() * moveSpeed * Time.deltaTime;
    }

    public void UpdateWeaponStats(CharacterStats _statsManager)
    {
        float moveSpeedPercent = _statsManager.GetStatValue(Stat.MoveSpeed) / 100;
        moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    }
}
