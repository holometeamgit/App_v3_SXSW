using Beem.Record.Video;
using Beem.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


/// <summary>
/// VideoPlayer Searcher
/// </summary>
public class VideoPlayerSearcher : MonoBehaviour {

    private void OnEnable() {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        AudioSource audioSourcer = GetComponent<AudioSource>();
        VideoPlayerCallBacks.onSetVideoPlayer?.Invoke(videoPlayer);
        VideoRecordCallbacks.onSetAudioSource?.Invoke(audioSourcer);
    }

}

