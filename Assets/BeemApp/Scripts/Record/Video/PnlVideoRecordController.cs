using UnityEngine;
using UnityEngine.Video;

namespace Beem.Record.Video {
    /// <summary>
    /// PnlVideoController
    /// </summary>
    public class PnlVideoRecordController : MonoBehaviour {
        [Header("Video Record Panel")]
        [SerializeField]
        private GameObject _pnl;
        [Header("Video player")]
        [SerializeField]
        private VideoPlayer _videoPlayer;

        private void OnEnable() {
            VideoRecordCallbacks.onRecordFinished += OnRecordComplete;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onRecordFinished -= OnRecordComplete;
        }

        private void OnRecordComplete(string outpath) {
            _pnl.SetActive(true);
            _videoPlayer.url = outpath;
            _videoPlayer.Play();
        }
    }
}