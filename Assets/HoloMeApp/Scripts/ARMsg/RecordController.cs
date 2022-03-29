using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.ARMsg {

    /// <summary>
    /// Timer Controller for Record Beem me
    /// </summary>
    public class RecordController : MonoBehaviour {
        [SerializeField]
        private UserWebManager _userWebManager;

        private List<int> _timers = new List<int> { 15, 30, 45, 60, 10 };
        private int _currentTimerID = 0;
        private const int TIME_FOR_SUPER_USER = 300;
        private const string SUPER_USER_CAPABILITY = "PC__AR_BEEM_UP_TO_5_MINS";

        /// <summary>
        /// Switch Timer
        /// </summary>
        public void SwitchTimer() {
            _currentTimerID = (_currentTimerID + 1) % _timers.Count;
            CheckForSuperUserTimer();

            CallBacks.onGetCurrevRecordTimerClicked?.Invoke();
        }

        /// <summary>
        /// Setup Timer
        /// </summary>
        public void OnGetCurrentRecordTimer() {
            CheckForSuperUserTimer();
        }

        private void Start() {
            CheckForSuperUserTimer();
            Beem.SSO.CallBacks.onUserDataLoaded += CheckForSuperUserTimer;
        }

        private void CheckForSuperUserTimer() {
            var capability = _userWebManager.GetCapabilities();

            if (capability != null && capability.Contains(SUPER_USER_CAPABILITY))
                CallBacks.onRecordTimerSet?.Invoke(TIME_FOR_SUPER_USER);
            else
                CallBacks.onRecordTimerSet?.Invoke(_timers[_currentTimerID]);
        }

        private void OnDestroy() {
            Beem.SSO.CallBacks.onUserDataLoaded -= CheckForSuperUserTimer;
        }
    }

}