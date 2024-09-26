using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveTransitionManager : MonoBehaviour, IGameStateListener
{
    [Header("ELEMENTS:")]
    [SerializeField] private Button[] upgradeContainers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStateChangedCallback(GameState _gameState)
    {
        switch (_gameState)
        {
            case GameState.WAVETRANSITION:
                ConfigureUpgradeContainers();
                break;
        }
    }

    [Button]
    private void ConfigureUpgradeContainers()
    {
        for (int i = 0; i < upgradeContainers.Length; i++)
        {

            int randomStat = Random.Range(0, Enum.GetValues(typeof(PlayerStat)).Length);

            PlayerStat playerStat = (PlayerStat)Enum.GetValues(typeof(PlayerStat)).GetValue(randomStat);

            string randomStatString = Enums.FormatStatName(playerStat);

            upgradeContainers[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = randomStatString;

            upgradeContainers[i].onClick.RemoveAllListeners();
            upgradeContainers[i].onClick.AddListener(() => Debug.Log(randomStatString));   
        }
    }

}
