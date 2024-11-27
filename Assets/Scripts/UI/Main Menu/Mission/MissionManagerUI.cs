using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public class MissionManagerUI : MonoBehaviour
    {
            [Header("ELEMENTS:")]
        [SerializeField] private MissionContainerUI missionContainerPrefab;
        [SerializeField] private Transform missionContainersParent;

        public void Init(Mission[] _activeMissions)
        {
            for (int i = 0; i < _activeMissions.Length; i++)
            {
                MissionContainerUI containerInstance = Instantiate(missionContainerPrefab, missionContainersParent);
            }
        }
    }

}
