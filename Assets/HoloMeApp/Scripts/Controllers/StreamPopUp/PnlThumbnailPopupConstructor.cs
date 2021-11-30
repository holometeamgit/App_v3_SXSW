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
        StreamCallBacks.onReceiveStreamLink += OpenStream;
    }

    private void OpenStream(string idString) {
        long id = 0;
        long.TryParse(idString, out id);
        _pnlThumbnailPopup.OpenStream(id);
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveStreamLink -= OpenStream;
    }
}
