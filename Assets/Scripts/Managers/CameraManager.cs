using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour, IGameModeListener
{
    public static CameraManager Instance;

    [Header("ELEMENTS:")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private int mainCameraOrthSize;
    [SerializeField] private int survivalCameraOrthSize;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private GameObject boardedMap;
    [SerializeField] private InfiniteMap infiniteMap;

    public bool UseInfiniteMap {get; private set;}

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() => SwitchToMainCamera();

    private void SwitchToMainCamera() 
    {
        UseInfiniteMap = false;
        boardedMap.SetActive(true);
        virtualCamera.Lens.OrthographicSize = mainCameraOrthSize;
        confiner2D.enabled = true;  
    }

    private void SwitchToSurvivalCamera()
    {
        UseInfiniteMap = true;
        infiniteMap.GenerateMap();
        boardedMap.SetActive(false);
        virtualCamera.Lens.OrthographicSize = survivalCameraOrthSize;
        confiner2D.enabled = false;
    }

    public void GameModeChangedCallback(GameMode _gameMode)
    {
        switch(_gameMode)
        {
            case GameMode.WaveBased:
                SwitchToMainCamera();
                break;
            case GameMode.BossRush:
                SwitchToMainCamera();
                break;
            case GameMode.ObjectiveBased:
                SwitchToMainCamera();
                break;
            case GameMode.Endless:
                SwitchToMainCamera();
                break;
            case GameMode.Survival:
                SwitchToSurvivalCamera();
                break;

        }
    }


}
