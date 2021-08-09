namespace Beem.Extenject.Record {
    /// <summary>
    /// Record End Signal
    /// </summary>
    public class RecordEndSignal : BeemSignal {

        private bool _success;

        public bool Success {
            get {
                return _success;
            }
        }

        public RecordEndSignal(bool success) {
            _success = success;
        }
    }
}
