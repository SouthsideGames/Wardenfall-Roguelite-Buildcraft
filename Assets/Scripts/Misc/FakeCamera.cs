using UnityEngine;

public class FakeCamera : MonoBehaviour
{
   
    [Header("Target to Look At")]
    public Transform target;

    [Header("Rotation Settings")]
    public RotationMode rotationMode = RotationMode.SmoothLerp;
    public float rotationSpeed = 5f;         // Used for SmoothLerp
    public float leanTweenTime = 0.2f;       // Used for LeanTween

    [Header("Per-Camera Rotation Offset")]
    public float rotationOffset = -90f;

    [Header("Rotation Limits")]
    public bool useRotationLimits = false;
    public float minAngle = -60f;
    public float maxAngle = 60f;

    private LTDescr currentRotationTween;

    private void Update()
    {
        if (target == null) return;

        // Direction to target
        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude < 0.001f) return;

        float rawAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = rawAngle + rotationOffset;

        // Apply clamping if limits are enabled
        if (useRotationLimits)
        {
            adjustedAngle = Mathf.Clamp(adjustedAngle, minAngle, maxAngle);
        }

        Quaternion desiredRotation = Quaternion.Euler(0, 0, adjustedAngle);

        // Apply rotation
        switch (rotationMode)
        {
            case RotationMode.Instant:
                transform.rotation = desiredRotation;
                break;

            case RotationMode.SmoothLerp:
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
                break;

            case RotationMode.LeanTween:
#if LEANTWEEN
                if (currentRotationTween != null) LeanTween.cancel(gameObject);
                currentRotationTween = LeanTween.rotate(gameObject, desiredRotation.eulerAngles, leanTweenTime).setEaseOutQuad();
#else
                Debug.LogWarning("LeanTween mode selected, but LeanTween is not installed!");
#endif
                break;
        }
    }
}
