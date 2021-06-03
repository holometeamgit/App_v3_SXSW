using UnityEngine;
using UnityEngine.Video;

namespace Beem.Record.Video {


    /// <summary>
    /// VideoRecord View
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoRecordView : MonoBehaviour {
        private VideoPlayer _videoPlayer;

        private void OnEnable() {
            _videoPlayer = GetComponent<VideoPlayer>();
            VideoRecordCallbacks.onRecordFinished += OnRecordComplete;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onRecordFinished -= OnRecordComplete;
        }

        private void OnRecordComplete(string outpath) {
            _videoPlayer.url = outpath;
            _videoPlayer.Play();
        }
    }
}
