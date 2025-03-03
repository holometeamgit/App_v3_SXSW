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
    private GameObject _empty;
    [SerializeField]
    private GameObject _notEmpty;
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private UserWebManager _userWebManager;
    private BusinessProfileManager _businessProfileManager;
    private WebRequestHandler _webRequestHandler;

    SignalBus _signalBus;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, UserWebManager userWebManager, BusinessProfileManager businessProfileManager, SignalBus signalBus) {
        _userWebManager = userWebManager;
        _businessProfileManager = businessProfileManager;
        _webRequestHandler = webRequestHandler;
        _signalBus = signalBus;
    }

    /// <summary>
    /// Show all elements
    /// </summary>
    /// <param name="arMsgJSON"></param>
    public void Show(ARMsgJSON arMsgJSON) {
        gameObject.SetActive(true);

        if (arMsgJSON.count > 0) {
            arMsgJSON.results.RemoveAll(x => x.processing_status == ARMsgJSON.Data.FAILED_STATUS);
            arMsgJSON.results.Sort((x, y) => -x.CreatedAt.CompareTo(y.CreatedAt));
        }

        _content.ClearContent();

        if (arMsgJSON.results.Count > 0) {
            _empty.SetActive(false);
            _notEmpty.SetActive(true);
            List<ScrollItemData> contentDatas = new List<ScrollItemData>();
            for (int i = 0; i < arMsgJSON.results.Count; i++) {
                ARMsgScrollItem aRMsgScrollItem = new ARMsgScrollItem(i);
                aRMsgScrollItem.Init(arMsgJSON.results[i], _userWebManager, _businessProfileManager, _webRequestHandler, GalleryNotificationController.IsNew(arMsgJSON.results[i]));
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

    private void Show(GetAllArMessagesSuccesSignal signal) {
        Show(signal.arMsgJSON);
    }

    private void RefreshWindow(ARMsgJSON.Data data) {
        _signalBus.Fire<GetAllArMessagesSignal>();
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        GalleryNotificationController.OnShow += RefreshWindow;
        _signalBus.Subscribe<GetAllArMessagesSuccesSignal>(Show);
    }

    private void OnDisable() {
        GalleryNotificationController.OnShow -= RefreshWindow;
        _signalBus.Unsubscribe<GetAllArMessagesSuccesSignal>(Show);
    }

}
