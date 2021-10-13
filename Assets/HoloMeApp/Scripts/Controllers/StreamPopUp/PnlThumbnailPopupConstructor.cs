using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlThumbnailPopupConstructor : MonoBehaviour {
    [SerializeField]
    private GameObject _pnlThumbnailPopupGO;
    private PnlThumbnailPopup _pnlThumbnailPopup;

    private void Awake() {
        Construct();
    }

    private void Construct() {
        StreamCallBacks.onStreamLinkReceived += OpenStream;
    }

    private void OpenStream(string idString) {

        _pnlThumbnailPopup = _pnlThumbnailPopupGO.GetComponent<PnlThumbnailPopup>();
        long id = 0;
        Debug.Log("OpenStream id parsed: " + long.TryParse(idString, out id));

        Debug.Log("OpenStream id : " + id);

        Debug.Log("_pnlThumbnailPopup is null : " + (_pnlThumbnailPopup == null));
        Debug.Log("_pnlThumbnailPopup  : " + _pnlThumbnailPopup);

        _pnlThumbnailPopup.OpenStream(id);
    }

    private void OnDestroy() {
        StreamCallBacks.onStreamLinkReceived -= OpenStream;
    }
}
