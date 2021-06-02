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
        [Header("Snap shot")]
        [SerializeField]
        private RawImage _snapshot;

        public void OnPointerDown(PointerEventData eventData) {
            if (_snapshot.texture != null) {
                new NativeShare().AddFile((Texture2D)_snapshot.texture).Share();
            } else {
                Debug.LogError("Screenshot was null");
            }
        }
    }
}
