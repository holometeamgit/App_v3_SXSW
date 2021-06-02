using UnityEngine;
using UnityEngine.UI;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// PnlSnapShotController
    /// </summary>
    public class PnlSnapShotController : MonoBehaviour {

        [Header("Snap shot Panel")]
        [SerializeField]
        private GameObject _pnl;
        [Header("Raw Image")]
        [SerializeField]
        private RawImage _rawImage;

        private void OnEnable() {
            SnapShotCallBacks.onSnapshotEnded += OnRecordComplete;
        }

        private void OnDisable() {
            SnapShotCallBacks.onSnapshotEnded -= OnRecordComplete;
        }

        private void OnRecordComplete(Texture2D screenshot) {
            _pnl.SetActive(true);
            _rawImage.texture = screenshot;
        }
    }
}
