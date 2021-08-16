using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Start Signal
    /// </summary>
    public class VideoRecordStartSignal : BeemSignal {
        private Vector2 _recordingTime;

        public Vector2 RecordingTime {
            get {
                return _recordingTime;
            }
        }

        public VideoRecordStartSignal(Vector2 recordingTime) {
            _recordingTime = recordingTime;
        }
    }
}
