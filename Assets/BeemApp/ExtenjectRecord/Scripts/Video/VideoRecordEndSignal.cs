namespace Beem.Extenject.Record {
    /// <summary>
    /// Record End Signal
    /// </summary>
    public class VideoRecordEndSignal : BeemSignal {

        private bool _success;

        public bool Success {
            get {
                return _success;
            }
        }

        public VideoRecordEndSignal(bool success) {
            _success = success;
        }
    }
}
