using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("ELEMENTS:")]
    [SerializeField] private Transform target;

    [Header("SETTINGS:")]
    [SerializeField] private Vector2 minMaxXY;

    private void LateUpdate()
    {
        if(target == null)
        {
            Debug.LogWarning("No target has been found");
            return;
        }

        Vector3 targetPosition = target.position;   
        targetPosition.z = -10;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -minMaxXY.x, minMaxXY.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -minMaxXY.y, minMaxXY.y);

        transform.position = targetPosition;   
    }
}
