using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Notification Constructor
/// </summary>
public class GalleryNotificationConstructor : WindowConstructor {
    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        GalleryNotificationController.OnShow += Show;
        GalleryNotificationController.OnHide += Hide;
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= Show;
        GalleryNotificationController.OnHide -= Hide;
        OnShow -= Show;
        OnHide -= Hide;
    }

    //TODO: Open Notification banner. It will be returned in next release
    private void Show() {
        //_window.SetActive(true);
    }

    private void Hide() {
        _window.SetActive(false);
    }
}
