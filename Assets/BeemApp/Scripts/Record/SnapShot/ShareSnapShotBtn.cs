using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Beem.Record.SnapShot {

    /// <summary>
    /// Share snapshot
    /// </summary>
    public class ShareSnapShotBtn : MonoBehaviour, IPointerDownHandler {

        private Texture2D _snapshot;

        private void OnEnable() {
            SnapShotCallBacks.onSnapshotEnded += OnRecordComplete;
        }

        private void OnDisable() {
            SnapShotCallBacks.onSnapshotEnded -= OnRecordComplete;
        }

        private void OnRecordComplete(Texture2D snapshot) {
            _snapshot = snapshot;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (_snapshot != null) {
                new NativeShare().AddFile(_snapshot).Share();
            } else {
                Debug.LogError("Screenshot was null");
            }
        }
    }
}
