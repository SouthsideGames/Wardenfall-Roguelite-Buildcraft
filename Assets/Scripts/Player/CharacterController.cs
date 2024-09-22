using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private MobileJoystick joystick;
    private Rigidbody2D _rb;

    [Header("SETTINGS:")]
    [SerializeField] private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); 
    }

    private void FixedUpdate()
    {
        _rb.velocity = joystick.GetMoveVector() * moveSpeed * Time.deltaTime;
    }
}
