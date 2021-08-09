using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Progress Signal
    /// </summary>
    public class RecordProgressSignal : BeemSignal {

        private float _progress = default;

        public float Progress {
            get {
                return _progress;
            }
        }

        public RecordProgressSignal(float progress) {
            _progress = progress;
        }
    }
}
