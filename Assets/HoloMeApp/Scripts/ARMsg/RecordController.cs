using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.ARMsg {

    /// <summary>
    /// Timer Controller for Record Beem me
    /// </summary>
    public class RecordController : MonoBehaviour {
        private List<int> _timers = new List<int> { 15, 30, 45, 60, 10};
        private int _currentTimerID = 0;

        /// <summary>
        /// Switch Timer
        /// </summary>
        public void SwitchTimer() {
            _currentTimerID = (_currentTimerID + 1) % _timers.Count;

            CallBacks.onGetCurrevRecordTimerClicked?.Invoke();
        }

        /// <summary>
        /// Setup Timer
        /// </summary>
        public void OnGetCurrentRecordTimer() {
            CallBacks.onRecordTimerSet?.Invoke(_timers[_currentTimerID]);
        }

        private void Start() {
            CallBacks.onRecordTimerSet?.Invoke(_timers[_currentTimerID]);
        }
    }

}