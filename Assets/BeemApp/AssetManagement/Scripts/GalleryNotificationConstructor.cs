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
        _galleryNotificationWindow.Show(data);
    }

    private void Hide() {
        _galleryNotificationWindow.Hide();
    }
}
