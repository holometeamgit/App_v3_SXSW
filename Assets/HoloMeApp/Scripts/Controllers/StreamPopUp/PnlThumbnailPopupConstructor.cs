using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlThumbnailPopupConstructor : MonoBehaviour {
    [SerializeField]
    private PnlThumbnailPopup _pnlThumbnailPopup;

    private void Awake() {
        Construct();
    }

    private void Construct() {
        StreamCallBacks.onStreamDataReceived += OpenStream;
    }

    private void OpenStream(StreamJsonData.Data data) {
        _pnlThumbnailPopup.OpenStream(data.id);
    }

    private void OnDestroy() {
        StreamCallBacks.onStreamDataReceived -= OpenStream;
    }
}
