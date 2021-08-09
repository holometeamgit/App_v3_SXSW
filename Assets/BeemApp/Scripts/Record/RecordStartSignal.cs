using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Start Signal
    /// </summary>
    public class RecordStartSignal : BeemSignal {
        private Vector2 _recordingTime;

        public Vector2 RecordingTime {
            get {
                return _recordingTime;
            }
        }

        public RecordStartSignal(Vector2 recordingTime) {
            _recordingTime = recordingTime;
        }
    }
}
