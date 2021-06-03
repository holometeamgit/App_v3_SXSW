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

        private void OnEnable() {
            VideoRecordCallbacks.onPostRecord += OnRecordComplete;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onPostRecord -= OnRecordComplete;
        }

        private void OnRecordComplete() {
            _pnl.SetActive(true);
        }
    }
}