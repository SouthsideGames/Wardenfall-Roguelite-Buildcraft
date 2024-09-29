using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour, ICharacterStats
{
    [Header("ELEMENTS:")]
    [SerializeField] private MobileJoystick joystick;
    private Rigidbody2D _rb;

    [Header("SETTINGS:")]
    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); 
    }

    private void FixedUpdate()
    {
        _rb.velocity = joystick.GetMoveVector() * moveSpeed * Time.deltaTime;
    }

    public void UpdateStats(CharacterStatsManager _characterStatsManager)
    {
        float moveSpeedPercent = _characterStatsManager.GetStatValue(CharacterStat.MoveSpeed) / 100;
        moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    }
}
