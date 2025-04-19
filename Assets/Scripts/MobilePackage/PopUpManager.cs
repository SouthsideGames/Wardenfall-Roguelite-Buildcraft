using System;
using System.Collections.Generic;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public class PopUpManager : MonoBehaviour 
    {
        public static PopUpManager Instance;

        [Header("ELEMENTS:")]
        [SerializeField] private Canvas popupCanvas;

        [Header("Data")]
        private List<PopupBaseUI> activePopUpList = new List<PopupBaseUI>();

        [Header("ACTIONS")]
        public static Action popUpOpened;
        public static Action allPopUpsClosed;

        private void Awake() 
        {
            if(Instance == null)
              Instance = this;
            else
                Destroy(gameObject);

            PopupBaseUI.closed += OnPopUpClosed;
        }

        void OnDestroy()
        {
            PopupBaseUI.closed -= OnPopUpClosed;
        }

        private void OnPopUpClosed(PopupBaseUI popup)
        {
            Instance.activePopUpList.Remove(popup);

            if(Instance.activePopUpList.Count > 0)
               return;

            allPopUpsClosed?.Invoke();
        }

        public static PopupBaseUI Show(PopupBaseUI popup)
        {
            PopupBaseUI popupInstance = Instantiate(popup, Instance.popupCanvas.transform);

            Instance.activePopUpList.Add(popupInstance);

            popUpOpened?.Invoke();  

            return popupInstance;
        }
    }
}