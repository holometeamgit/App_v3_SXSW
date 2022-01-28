using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.ARMsg {

    public class RecordController : MonoBehaviour {
        [SerializeField]
        private List<int> _timers = new List<int> { 10, 15, 30, 45, 60 };
        private int _currentTimerID = 0;

        public void SwitchTimer() {
            _currentTimerID = (_currentTimerID + 1) % _timers.Count;

            CallBacks.onGetCurrevRecordTimerClicked?.Invoke();
        }

        public void onGetCurrevRecordTimer() {
            CallBacks.onRecordTimerSet?.Invoke(_timers[_currentTimerID]);
        }
    }

}