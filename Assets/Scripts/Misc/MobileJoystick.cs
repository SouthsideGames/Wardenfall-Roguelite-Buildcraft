using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileJoystick : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private RectTransform joystickOutline;
    [SerializeField] private RectTransform joystickKnob;

    [Header("SETTINGS:")]
    [SerializeField] private float moveFactor = 1f;
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private bool useHaptics = true;
    private Vector3 clickedPosition;
    private Vector3 move;
    private bool canControl;

    // Start is called before the first frame update
    void Start()
    {
        HideJoystick();
    }

    private void OnDisable()
    {
        HideJoystick();
    }

    // Update is called once per frame
    void Update()
    {
        if(canControl)
            ControlJoystick();
    }

    public void ClickedOnJoystickZoneCallback()
    {
        clickedPosition = Input.mousePosition;
        joystickOutline.position = clickedPosition;

        ShowJoystick();
    }

    private void ShowJoystick()
    {
        joystickOutline.gameObject.SetActive(true);
        canControl = true;
    }

    private void HideJoystick()
    {
        joystickOutline.gameObject.SetActive(false);
        canControl = false;

        move = Vector3.zero;
    }

    private void ControlJoystick()
    {
        Vector3 currentPosition = Input.mousePosition;
        Vector3 direction = currentPosition - clickedPosition;

        float canvasScale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x;
        
        // Apply deadzone
        float rawMagnitude = direction.magnitude;
        float normalizedMagnitude = Mathf.Max(0, rawMagnitude - (deadZone * canvasScale)) / (1 - deadZone);
        
        float moveMagnitude = normalizedMagnitude * moveFactor * canvasScale;

        if (useHaptics && rawMagnitude > deadZone * canvasScale)
        {
            HapticFeedbackUI.Trigger();
        }

        float absoluteWidth = joystickOutline.rect.width / 2;
        float realWidth = absoluteWidth * canvasScale;

        moveMagnitude = Mathf.Min(moveMagnitude, realWidth);

        move = direction.normalized;

        Vector3 knobMove = move * moveMagnitude;
        
        Vector3 targetPosition = clickedPosition + knobMove;

        joystickKnob.position = targetPosition;

        if (Input.GetMouseButtonUp(0))
            HideJoystick();
    }

    public Vector3 GetMoveVector() => move;
}
