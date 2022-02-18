using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeepLink Stream Costructor
/// </summary>
public class DeepLinkStreamConstructor : MonoBehaviour {
    [SerializeField]
    private DeepLinkStreamPopup _pnlThumbnailPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    public static Action<StreamJsonData.Data> OnShow = delegate { };
    public static Action<WebRequestError> OnShowError = delegate { };
    public static Action OnHide = delegate { };
    private void OnEnable() {
        OnShow += Show;
        OnShowError += ShowError;
        OnHide += Hide;
    }

    private void Show(StreamJsonData.Data data) {
        _popupShowChecker.OnReceivedData(data, ActivatePopup);
    }

    private void ActivatePopup(StreamJsonData.Data data) {
        _pnlThumbnailPopup.Show(data);
    }

    private void Hide() {
        _pnlThumbnailPopup.Hide();
    }

    private void OnDisable() {
        OnShow -= Show;
        OnShowError -= ShowError;
        OnHide -= Hide;
    }

    private void ShowError(WebRequestError webRequestError) {
        _popupShowChecker.OnReceivedData(() => WarningConstructor.ActivateSingleButton("This page doesn't exist", "Please make sure that the link you received is correct.", "Ok"));
    }
}
