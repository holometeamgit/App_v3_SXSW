using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Beem.Video {

    /// <summary>
    /// Timer View
    /// </summary>
    public class VideoPlayerTimerView : MonoBehaviour {
        [Header("Video Player")]
        [SerializeField]
        private VideoPlayer videoPlayer;

        private Text timerText;

        private CancellationTokenSource cancelTokenSource;

        private void Awake() {
            timerText = GetComponent<Text>();
        }

        private void Start() {
            UpdateTimer();
        }

        public async void UpdateTimer() {
            timerText.text = string.Empty;
            cancelTokenSource = new CancellationTokenSource();
            try {
                while (true) {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(videoPlayer.frame);
                    if (timerText != null) {
                        timerText.text = string.Concat(timeSpan);
                    }
                    await Task.Yield();
                }
            }
            finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        private void OnDestroy() {
            Clear();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        public void Clear() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }
    }
}
