using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Video {
    /// <summary>
    /// VideoPlayer Searcher
    /// </summary>
    public class VideoPlayerSearcher : MonoBehaviour {

        private void OnEnable() {
            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            VideoPlayerCallBacks.onSetVideoPlayer?.Invoke(videoPlayer);
        }

    }
}
