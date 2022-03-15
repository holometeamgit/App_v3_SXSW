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
        RefreshBadge();
        GalleryNotificationController.OnShow += RefreshBadge;
        GalleryNotificationController.OnHide += RefreshBadge;
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= RefreshBadge;
        GalleryNotificationController.OnHide -= RefreshBadge;
    }

    private void RefreshBadge() {
        _badge.SetActive(GalleryNotificationController.ContainsNew());
    }
}
