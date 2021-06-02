using System;
using UnityEngine;
namespace Beem.Record.Video {
    /// <summary>
    /// Video Callbacks
    /// </summary>
    public class VideoRecordCallbacks {
        public static Action onRecordStarted = delegate { };
        public static Action<float> onRecordProgress = delegate { };
        public static Action onRecordFailed = delegate { };
        public static Action onRecordStoped = delegate { };
        public static Action<string> onRecordFinished = delegate { };
        public static Action<AudioSource> onSetAudioSource = delegate { };
    }
}
