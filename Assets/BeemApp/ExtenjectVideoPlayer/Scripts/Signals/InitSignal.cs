using UnityEngine.Video;

namespace Beem.Extenject.Video {
    /// <summary>
    /// Set Video Signal
    /// </summary>
    public class InitSignal : BeemSignal {
        private VideoPlayer _player;

        public VideoPlayer Player {
            get {
                return _player;
            }
        }

        public InitSignal(VideoPlayer player) {
            _player = player;
        }
    }
}
