using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileJoystick : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private RectTransform joystickOutline;
    [SerializeField] private RectTransform joystickKnob;

    [Header("SETTINGS:")]
    [SerializeField] private float moveFactor;


    private Vector3 clickedPosition;
    private Vector3 move;
    private bool canControl = true;

    // Start is called before the first frame update
    void Start()
    {
        HideJoystick();
    }

    // Update is called once per frame
   void Update()
    {
        Debug.Log($"[Joystick] Update running: canControl = {canControl}");

        if (canControl)
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
        joystickKnob.gameObject.SetActive(true);

        canControl = true;
    }

    private void HideJoystick()
    {
        joystickOutline.gameObject.SetActive(false); // Only hide visuals
        joystickKnob.gameObject.SetActive(false);    // Also hide knob if needed

        canControl = false;
        move = Vector3.zero;
    }
    private void ControlJoystick()
    {
        Vector3 currentPosition = Input.mousePosition;
        Vector3 direction = currentPosition - clickedPosition;

        float canvasScale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x;
        float moveMagnitude = direction.magnitude * moveFactor * canvasScale;
        float absoluteWidth = joystickOutline.rect.width / 2;
        float realWidth = absoluteWidth * canvasScale;

        moveMagnitude = Mathf.Min(moveMagnitude, realWidth);

        move = direction.normalized;

        Vector3 knobMove = move * moveMagnitude;

        Vector3 targetPosition = clickedPosition + knobMove;

        joystickKnob.position = targetPosition;

        if (Input.GetMouseButtonUp(0))
            HideJoystick();

              Debug.Log($"Joystick knob moved to: {targetPosition}");
    }

    public Vector3 GetMoveVector() => move;
}