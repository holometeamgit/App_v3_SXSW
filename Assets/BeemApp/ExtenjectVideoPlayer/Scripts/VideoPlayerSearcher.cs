using UnityEngine;
using UnityEngine.Video;

namespace Beem.Extenject.Video {

    /// <summary>
    /// VideoPlayer Searcher
    /// </summary>
    public class VideoPlayerSearcher : MonoBehaviour {

        private void Start() {
            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            VideoPlayerController.onSetVideoPlayer?.Invoke(videoPlayer);
        }

    }
}

