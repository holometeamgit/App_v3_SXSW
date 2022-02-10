using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkStreamConstructor : MonoBehaviour {
    [SerializeField]
    private PnlThumbnailPopup _pnlThumbnailPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    public static Action<StreamJsonData.Data> OnShow = delegate { };
    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void Awake() {
        Construct();
    }

    private void Construct() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void Show(StreamJsonData.Data data) {
        _popupShowChecker.OnReceivedData(data, ActivatePopup);
    }

    private void ActivatePopup(StreamJsonData.Data data) {
        IsActive = true;
        _pnlThumbnailPopup.Show(data);
    }

    private void Hide() {
        IsActive = false;
        _pnlThumbnailPopup.Hide();
    }

    private void OnDestroy() {
        OnShow -= Show;
        OnHide -= Hide;
    }
}
