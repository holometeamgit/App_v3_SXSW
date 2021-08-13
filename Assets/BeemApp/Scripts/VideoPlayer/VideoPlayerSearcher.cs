using Beem.Video;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// VideoPlayer Searcher
/// </summary>
public class VideoPlayerSearcher : MonoBehaviour {

    private void Start() {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        VideoPlayerController.onSetVideoPlayer?.Invoke(videoPlayer);
    }

}

