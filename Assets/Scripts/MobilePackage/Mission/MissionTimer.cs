using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;


namespace SouthsideGames.DailyMissions
{
    using SouthsideGames.SaveManager;

    public class MissionTimer : MonoBehaviour
    {
        [Header("ELEMENTS:")]
        [SerializeField] private TextMeshProUGUI timerText;
        private MissionManager missionManager;

        [Header("DATA:")]
        private DateTime startTime;
        private DateTime endTime;
        private const string startTimeKey   = "DailyMissionStartTime";
        private const string endTimeKey     = "DailyMissionEndTime";

        public bool TimerIsActive { get; private set; }

        private TimeSpan TimeLeft
        {
            get { return endTime - DateTime.UtcNow; }
            set { }
        }

        public void Init(MissionManager _missionManager)
        {
            this.missionManager = _missionManager;

            LoadTimes();

            if(startTime == DateTime.MinValue || endTime == DateTime.MinValue)
               StartTimer();
            else
                ConfigureTimer();
        }

        public void StartTimer()
        {
            startTime = DateTime.UtcNow;
            endTime = startTime + TimeSpan.FromDays(1);

            SaveTimes();

            UpdateTimerText();

            StartCoroutine("TimerCoroutine");

            TimerIsActive = true;
        }

        public void ConfigureTimer()
        {
            if(endTime.CompareTo(DateTime.UtcNow) <= 0)
            {
                DayEnd();
                return;
            }

            TimerIsActive = true;

            UpdateTimerText();

            StartCoroutine("TimerCoroutine");
        }

        private IEnumerator TimerCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(10);

                SaveTimes();

                if(endTime.CompareTo(DateTime.UtcNow) <= 0)
                {
                    DayEnd();
                    yield break;    
                }

                UpdateTimerText();

            }
        }

        public void AddHours(int _hours)
        {
            endTime -= TimeSpan.FromHours(_hours);
            SaveTimes();

            if(endTime.CompareTo(DateTime.UtcNow) <= 0)
            {
                DayEnd();
                return;
            }

            UpdateTimerText();
        }

        private void DayEnd()
        {
            StopAllCoroutines();
            missionManager.ResetMissions();

            TimerIsActive = false;
        }

        private void SaveTimes()
        {
            SaveManager.Save(this, startTimeKey, startTime.ToString());
            SaveManager.Save(this, endTimeKey, endTime.ToString());
        }

        private void LoadTimes()
        {
            if(SaveManager.TryLoad(this, startTimeKey, out object _startTimer))
            {
                string startTimeString = (string) _startTimer;

                if(!DateTime.TryParse(startTimeString, out startTime))
                   Debug.LogWarning("Failed to parse the start timer");
            }

            if(SaveManager.TryLoad(this, endTimeKey, out object _endTimer))
            {
                string endTimeString = (string) _endTimer;

                if(!DateTime.TryParse(endTimeString, out endTime))
                   Debug.LogWarning("Failed to parse the end timer");
            }
        }

        private void UpdateTimerText()
        {
            timerText.text = CustomTimeSpanToString(TimeLeft);
        }

        public static string CustomTimeSpanToString(TimeSpan _timeLeft)
        {
            int days = _timeLeft.Days;
            int hours = _timeLeft.Hours;
            int minutes = _timeLeft.Minutes;

            string daysString = days > 0 ? days + "d" : "";
            string separation = hours > 0 ? " " : "";
            string hoursString = hours > 0 ? hours + "h" : "";

            bool showMinutesCondition = (days == 0 && hours >= 0) || (days >= 0 && hours == 0);

            string separationBis = showMinutesCondition ? " " : "";
            string minutesString = showMinutesCondition ? minutes + "min" : "";

            return daysString + separation + hoursString + separationBis + minutesString;

        }

        void OnApplicationPause(bool pause)
        {
            if(pause)
               SaveTimes();
        }

        public void ResetData()
        {

        }

        [Button]
        private void AddOneHour() => AddHours(1);

    }

}
