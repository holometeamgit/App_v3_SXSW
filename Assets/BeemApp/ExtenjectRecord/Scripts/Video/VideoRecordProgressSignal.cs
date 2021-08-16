namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Progress Signal
    /// </summary>
    public class VideoRecordProgressSignal : BeemSignal {

        private float _progress = default;

        public float Progress {
            get {
                return _progress;
            }
            set {
                _progress = value;
            }
        }
    }
}
