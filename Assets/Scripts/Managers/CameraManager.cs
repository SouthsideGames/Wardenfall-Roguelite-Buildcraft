using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public CinemachineCamera virtualCamera;

    private int originalOrthoSize = 8;
    private int currentOrthoSize;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start() => ResetCameraZoom();

    public void SetCameraZoom(int zoomLevel)
    {
        virtualCamera.Lens.OrthographicSize = zoomLevel;
    }

    private void ResetCameraZoom()
    {
        currentOrthoSize = originalOrthoSize;
        virtualCamera.Lens.OrthographicSize = currentOrthoSize;
    }   
}
