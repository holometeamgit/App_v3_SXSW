using UnityEngine;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Snap shot Finish Signal
    /// </summary>
    public class SnapShotFinishSignal : BeemSignal {
        private Texture2D _snapShot;

        public Texture2D SnapShot {
            get {
                return _snapShot;
            }
        }

        public SnapShotFinishSignal(Texture2D snapShot) {
            _snapShot = snapShot;
        }
    }
}
