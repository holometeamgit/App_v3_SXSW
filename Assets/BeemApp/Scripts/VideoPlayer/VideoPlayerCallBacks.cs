using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Video {
    /// <summary>
    /// VideoPlayer Callbacks
    /// </summary>
    public class VideoPlayerCallBacks {
        public static Action<VideoPlayer> onSetVideoPlayer;
        public static Action onPlay;
        public static Action onRewindStarted;
        public static Action<float> onRewind;
        public static Action<float> onRewindFinished;
        public static Action onPause;
    }
}