using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// VideoPlayer Searcher
    /// </summary>
    public class VideoPlayerSearcher : MonoBehaviour {

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Start() {
            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            _signalBus.Fire(new InitSignal(videoPlayer));
        }

    }
}

