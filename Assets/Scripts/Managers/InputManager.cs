using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private MobileJoystick joystick;

    [Header("SETTINGS:")]
    [SerializeField] private bool forceHandheld = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (SystemInfo.deviceType == DeviceType.Desktop && !forceHandheld)
            joystick.gameObject.SetActive(false);
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
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
