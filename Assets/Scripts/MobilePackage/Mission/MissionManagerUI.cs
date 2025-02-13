using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public class MissionManagerUI : MonoBehaviour
    {
        [Header("ELEMENTS:")]
        [SerializeField] private MissionContainerUI missionContainerPrefab;
        [SerializeField] private Transform missionContainersParent;

        List<MissionContainerUI> activeMissionContainers = new List<MissionContainerUI>();

        public void Init(Mission[] _activeMissions)
        {
            for (int i = 0; i < _activeMissions.Length; i++)
            {
                MissionContainerUI containerInstance = Instantiate(missionContainerPrefab, missionContainersParent);

                int _i = i;
                containerInstance.Configure(_activeMissions[i], () => ClaimMission(_i));

                activeMissionContainers.Add(containerInstance);
            }

            Reorder();
        }

        private void Reorder()
        {
            for(int i = 0; i < activeMissionContainers.Count; i++)
                if(activeMissionContainers[i].IsClaimed)
                    activeMissionContainers[i].transform.SetAsFirstSibling();
        }

        private void ClaimMission(int _index)
        {
            activeMissionContainers[_index].ClaimMission();
            activeMissionContainers[_index].transform.SetAsLastSibling();

            MissionManager.Instance.HandleMissionClaimed(_index);
        }

        public void UpdateMission(int _index) => activeMissionContainers[_index].UpdateVisuals(); 
    }

}
