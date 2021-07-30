using UnityEngine.Video;

namespace Beem.Extenject.Video {
    /// <summary>
    /// Video Signal
    /// </summary>
    public class VideoSignal : BeemSignal {

        public VideoPlayer Video {
            get {
                return _video;
            }
        }

        private VideoPlayer _video;

        public VideoSignal(VideoPlayer video) {
            _video = video;
        }
    }
}
