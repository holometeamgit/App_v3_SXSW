using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GalleryNotificationBtn
/// </summary>
public class GalleryNotificationBtn : MonoBehaviour {
    [SerializeField]
    private bool open;

    /// <summary>
    /// On Click element
    /// </summary>
    public void OnClick() {
        if (open) {
            GalleryNotificationController.OnShow?.Invoke();
        } else {
            GalleryNotificationController.OnHide?.Invoke();
        }
    }
}
