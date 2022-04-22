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
    private WebRequestHandler _webRequestHandler;

    private GetAllARMessageController _galleryController;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, UserWebManager userWebManager) {
        _userWebManager = userWebManager;
        _webRequestHandler = webRequestHandler;
        _galleryController = new GetAllARMessageController(_arMsgAPIScriptableObject, webRequestHandler);
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
                aRMsgScrollItem.Init(arMsgJSON.results[i], _userWebManager, _webRequestHandler, GalleryNotificationController.IsNew(arMsgJSON.results[i]));
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

    private void RefreshWindow(ARMsgJSON.Data data) {
        _galleryController.GetAllArMessages(onSuccess: Show);
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }
}
