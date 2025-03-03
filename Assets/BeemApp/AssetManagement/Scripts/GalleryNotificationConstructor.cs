using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Notification Constructor
/// </summary>
public class GalleryNotificationConstructor : MonoBehaviour {

    [SerializeField]
    private GalleryNotificationWindow _galleryNotificationWindow;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    public static Action OnHide = delegate { };

    private void OnEnable() {
        GalleryNotificationController.OnShow += Show;
        GalleryNotificationController.OnHide += Hide;
        OnHide += Hide;
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= Show;
        GalleryNotificationController.OnHide -= Hide;
        OnHide -= Hide;
    }

    private void Show(ARMsgJSON.Data data) {
        OnReceivedARMessageData(data, ActivateData);
    }

    private void OnReceivedARMessageData(ARMsgJSON.Data data, Action<ARMsgJSON.Data> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ActivateData(ARMsgJSON.Data data) {
        _galleryNotificationWindow.Show(data);
    }

    private void Hide() {
        _galleryNotificationWindow.Hide();
    }
}
