using DynamicScrollRect;
using Firebase.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Gallery View
/// </summary>
public class GalleryWindow : MonoBehaviour {
    [SerializeField]
    private ScrollContent _content = null;
    [SerializeField]
    private GameObject _pushNotificationPopUp;
    [SerializeField]
    private CanvasScaler _canvasScaler = null;
    [SerializeField]
    private RectTransform _scrollRect;
    [SerializeField]
    private Image _topGradient;
    [SerializeField]
    private GameObject _empty;
    [SerializeField]
    private GameObject _notEmpty;
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;
    [SerializeField]
    private float _topShift = 400f;

    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;

    private GetAllARMessageController _galleryController;
    private const string TOPIC = "gallery_{0}";

    private bool CanShowPushNotificationPopup {
        get {
            return PlayerPrefs.GetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), 1) == 1;
        }
        set {
            PlayerPrefs.SetInt("PushNotificationForARMessage" + _userWebManager?.GetUsername(), value ? 1 : 0);
        }
    }

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, UserWebManager userWebManager) {
        _webRequestHandler = webRequestHandler;
        _userWebManager = userWebManager;
    }

    private void Start() {
        _galleryController = new GetAllARMessageController(_arMsgAPIScriptableObject, _webRequestHandler);
    }

    private void OnEnable() {
        GalleryNotificationController.OnShow += RefreshWindow;
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= RefreshWindow;
    }

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public void Show(ARMsgJSON arMsgJSON) {
        gameObject.SetActive(true);
        RefreshScrollRectSize();

        if (arMsgJSON.count > 0) {
            _empty.SetActive(false);
            _notEmpty.SetActive(true);
            _content.ClearContent();
            List<ScrollItemData> contentDatas = new List<ScrollItemData>();
            arMsgJSON.results.RemoveAll(x => x.processing_status == ARMsgJSON.Data.FAILED_STATUS);
            arMsgJSON.results.Sort((x, y) => -x.CreatedAt.CompareTo(y.CreatedAt));
            for (int i = 0; i < arMsgJSON.results.Count; i++) {
                ARMsgScrollItem aRMsgScrollItem = new ARMsgScrollItem(i);
                aRMsgScrollItem.Init(arMsgJSON.results[i], GalleryNotificationController.IsNew(arMsgJSON.results[i]));
                contentDatas.Add(aRMsgScrollItem);
            }

            _content.InitScrollContent(contentDatas);
            _content.DynamicScrollRect.verticalNormalizedPosition = 1;
            GalleryNotificationController.Clear();
        } else {
            _empty.SetActive(true);
            _notEmpty.SetActive(false);
        }
    }

    private void RefreshScrollRectSize() {
        _pushNotificationPopUp.SetActive(CanShowPushNotificationPopup);

        RectTransform pushRect = _pushNotificationPopUp.GetComponent<RectTransform>();
        float shift = (CanShowPushNotificationPopup ? 1 : 0) * pushRect.sizeDelta.y + _topShift;
        _topGradient.fillAmount = shift / _canvasScaler.referenceResolution.y;

        Vector2 offsetMax = _scrollRect.offsetMax;
        offsetMax.y = -shift;
        _scrollRect.offsetMax = offsetMax;
    }

    private void RefreshWindow(ARMsgJSON.Data data) {
        _galleryController.GetAllArMessages(onSuccess: Show);
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
        CanShowPushNotificationPopup = false;
        RefreshScrollRectSize();
    }

    /// <summary>
    /// Decline Notification
    /// </summary>
    public void DeclineNotification() {
        CanShowPushNotificationPopup = false;
        RefreshScrollRectSize();
    }
}
