using DynamicScrollRect;
using Firebase.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gallery Notification View
/// </summary>
public class GalleryNotificationWindow : MonoBehaviour {

    private ARMsgJSON.Data _data;

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public void Show(ARMsgJSON.Data data) {
        gameObject.SetActive(true);
        _data = data;

    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Open Message in AR
    /// </summary>
    public void Open() {
        if (_data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            DeeplinkARMsgConstructor.OnShow(_data);
            Close();
        }
    }

    /// <summary>
    /// On Click element
    /// </summary>
    public void Close() {
        GalleryNotificationController.OnHide?.Invoke();
    }

}
