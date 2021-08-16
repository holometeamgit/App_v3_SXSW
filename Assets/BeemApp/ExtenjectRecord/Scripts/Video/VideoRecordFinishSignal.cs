namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Finish Signal
    /// </summary>
    public class VideoRecordFinishSignal : BeemSignal {

        private string _path;

        public string Path {
            get {
                return _path;
            }
        }

        public VideoRecordFinishSignal(string path) {
            _path = path;
        }
    }
}
