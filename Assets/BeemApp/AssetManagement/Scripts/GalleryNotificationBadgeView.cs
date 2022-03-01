using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Gallery Badge View
/// </summary>
public class GalleryNotificationBadgeView : MonoBehaviour {
    [SerializeField]
    private GameObject _badge;

    private void OnEnable() {
        GalleryNotificationController.OnShow += Show;
        GalleryNotificationController.OnHide += Hide;
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= Show;
        GalleryNotificationController.OnHide -= Hide;
    }

    private void Show() {
        _badge.SetActive(true);
    }

    private void Hide() {
        _badge.SetActive(false);
    }
}
