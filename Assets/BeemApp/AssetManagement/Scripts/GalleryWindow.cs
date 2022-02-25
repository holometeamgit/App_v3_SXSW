using DynamicScrollRect;
using Firebase.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Gallery View
/// </summary>
public class GalleryWindow : MonoBehaviour {
    [SerializeField]
    private ScrollContent _content = null;
    [SerializeField]
    private GameObject pushNotificationPopUp;
    [SerializeField]
    private GameObject _empty;
    [SerializeField]
    private GameObject _notEmpty;

    [Space]
    [SerializeField]
    private UserWebManager _userWebManager;

    private const string TOPIC = "gallery_{0}";

    private bool CanShowPushNotificationPopup {
        get {
            return PlayerPrefs.GetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), 1) == 1;
        }
        set {
            PlayerPrefs.SetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), value ? 1 : 0);
        }
    }

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public void Show(ARMsgJSON arMsgJSON) {
        gameObject.SetActive(true);
        pushNotificationPopUp.SetActive(CanShowPushNotificationPopup);

        if (arMsgJSON.count > 0) {
            _empty.SetActive(false);
            _notEmpty.SetActive(true);

            List<ScrollItemData> contentDatas = new List<ScrollItemData>();
            arMsgJSON.results.Sort((x, y) => x.processing_status.CompareTo(y.processing_status));
            for (int i = 0; i < arMsgJSON.count; i++) {
                ARMsgScrollItem aRMsgScrollItem = new ARMsgScrollItem(i);
                aRMsgScrollItem.Init(arMsgJSON.results[i], GalleryNotificationController.IsNew(arMsgJSON.results[i]));
                contentDatas.Add(aRMsgScrollItem);
            }

            _content.InitScrollContent(contentDatas);
            GalleryNotificationController.Clear();
        } else {
            _empty.SetActive(true);
            _notEmpty.SetActive(false);
        }
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Allow Notification
    /// </summary>
    public void AllowNotification() {
        FirebaseMessaging.SubscribeAsync(string.Format(TOPIC, _userWebManager?.GetUsername()));
        pushNotificationPopUp.SetActive(false);
        CanShowPushNotificationPopup = false;
    }

    /// <summary>
    /// Decline Notification
    /// </summary>
    public void DeclineNotification() {
        pushNotificationPopUp.SetActive(false);
        CanShowPushNotificationPopup = false;
    }
}
