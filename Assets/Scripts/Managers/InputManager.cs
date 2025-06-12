using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public static Action OnCancel;
    public static Action OnLock;
    public static Action<float> OnScroll;

    [Header("ELEMENTS:")]
    [SerializeField] private MobileJoystick joystick;
    [SerializeField] private GameObject joystickKnob;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private InputActionAsset actions;

    [Header("SETTINGS:")]
    [SerializeField] private bool forceHandheld = true;

    [Header("INPUT ACTIONS:")]
    private InputAction movementAction;
    private InputAction pauseAction;
    private InputAction cancelAction;
    private InputAction lockAction;
    private InputAction scrollViewAction;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        bool isMobile = Application.isMobilePlatform || forceHandheld;
        joystick.gameObject.SetActive(isMobile);
        pauseButton.gameObject.SetActive(isMobile);

        // Disable screen dimming on mobile
        if (isMobile)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }



        movementAction = actions.FindAction("Movement");
        pauseAction = actions.FindAction("Pause");
        cancelAction = actions.FindAction("Cancel");
        lockAction = actions.FindAction("Lock");
        scrollViewAction = actions.FindAction("ScrollView");

        pauseAction.performed += PauseCallback;
        cancelAction.performed += CancelCallback;
        lockAction.performed += LockCallback;
        scrollViewAction.performed += ScrollCallback;

        actions.Enable();
    }

    private void OnDestroy()
    {
        pauseAction.performed -= PauseCallback;
        cancelAction.performed -= CancelCallback;
        lockAction.performed -= LockCallback;
        scrollViewAction.performed -= ScrollCallback;
    }

    public Vector2 GetMoveVector()
    {
        if (SystemInfo.deviceType == UnityEngine.DeviceType.Desktop && !forceHandheld)
            return GetDesktopMoveVector();
        else
            return joystick.GetMoveVector();
    }

    private Vector2 GetDesktopMoveVector() => movementAction.ReadValue<Vector2>();
    private void PauseCallback(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.gameState == GameState.Game)
        {
            GameManager.Instance.PauseButtonCallback();
        }
    }

    private void ScrollCallback(InputAction.CallbackContext rightStick) => OnScroll?.Invoke(rightStick.ReadValue<float>());
    private void LockCallback(InputAction.CallbackContext context) => OnLock?.Invoke();
    private void CancelCallback(InputAction.CallbackContext context) => OnCancel?.Invoke(); 
    public void ActivateJoystick() => joystickKnob.SetActive(true);

}
