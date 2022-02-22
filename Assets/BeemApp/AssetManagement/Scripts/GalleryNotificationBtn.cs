using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GalleryNotificationBtn
/// </summary>
public class GalleryNotificationBtn : MonoBehaviour {
    [SerializeField]
    private bool open;

    public void OnClick() {
        if (open) {
            GalleryNotificationConstructor.OnShow?.Invoke();
        } else {
            GalleryNotificationConstructor.OnHide?.Invoke();
        }
    }
}
