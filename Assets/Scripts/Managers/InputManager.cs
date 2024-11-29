using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private MobileJoystick joystick;
    [SerializeField] private InputActionAsset actions;

    [Header("SETTINGS:")]
    [SerializeField] private bool forceHandheld = true;

    [Header("INPUT ACTIONS:")]
    private InputAction movement;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (SystemInfo.deviceType == DeviceType.Desktop && !forceHandheld)
            joystick.gameObject.SetActive(false);

        movement = actions.FindAction("Movement");

        actions.Enable();
    }

    public Vector2 GetMoveVector()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop && !forceHandheld)
            return GetDesktopMoveVector();
        else
            return joystick.GetMoveVector();
    }

    private Vector2 GetDesktopMoveVector()
    {
        return movement.ReadValue<Vector2>();
    }
}
