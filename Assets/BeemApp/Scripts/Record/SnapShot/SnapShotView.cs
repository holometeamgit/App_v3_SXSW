using Beem.Record.SnapShot;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// SnapShot View
    /// </summary>

    [RequireComponent(typeof(RawImage))]
    public class SnapShotView : MonoBehaviour {

        private RawImage _rawImage;

        private void OnEnable() {
            _rawImage = GetComponent<RawImage>();
            SnapShotCallBacks.onSnapshotEnded += OnRecordComplete;
        }

        private void OnDisable() {
            SnapShotCallBacks.onSnapshotEnded -= OnRecordComplete;
        }

        private void OnRecordComplete(Texture2D screenshot) {
            _rawImage.texture = screenshot;
        }
    }
}
