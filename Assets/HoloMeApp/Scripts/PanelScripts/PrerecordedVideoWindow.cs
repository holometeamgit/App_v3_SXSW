using Beem.Permissions;
using Beem.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel for Prerecorded Video
/// </summary>
public class PrerecordedVideoWindow : MonoBehaviour {

    [Header("Likes Counter")]
    [SerializeField]
    private StreamLikesRefresherView _streamLikesRefresherView;

    [Header("Views after hologram placement")]
    [SerializeField]
    private CanvasGroup _placementView;

    [Header("View for video")]
    [SerializeField]
    private CanvasGroup _videoView;

    [Header("Purchase for video")]
    [SerializeField]
    private CanvasGroup _purchaseView;

    [Header("Comments Toggle")]
    [SerializeField]
    private Toggle commentsToggle;

    [Space]
    [SerializeField]
    private PurchaseManager _purchaseManager;
    [SerializeField]
    private HologramHandler _hologramHandler;

    private StreamJsonData.Data _streamData = default;

    private List<IStreamDataView> _streamDataViewaViews;

    private bool newDataAssigned = false;

    private bool isPinned = false;


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        newDataAssigned = true;
        _streamData = streamData;

        gameObject.SetActive(true);

        _streamDataViewaViews = GetComponentsInChildren<IStreamDataView>().ToList();
        _streamDataViewaViews.ForEach(x => x.Init(_streamData));

        if (isPinned) {
            OnPlacementCompleted();
        } else {
#if !UNITY_EDITOR
            SetActive(_placementView, false);
#else
            OnPlacementCompleted();
#endif
        }

        Refresh();
        _hologramHandler.SetOnPlacementUIHelperFinished(OnPlacementCompleted);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }

    private void OnPlacementCompleted() {
        if (!newDataAssigned) {
            return;
        }
        isPinned = true;
        SetActive(_placementView, true);
        RecordARConstructor.OnActivated?.Invoke(_streamData.IsStarted && _streamData.is_bought);
    }

    private void SetActive(CanvasGroup canvasGroup, bool value) {
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    private void Refresh() {
        if (!newDataAssigned) {
            return;
        }
        SetActive(_placementView, !commentsToggle.isOn);
        SetActive(_videoView, _streamData.IsStarted && _streamData.is_bought);
        SetActive(_purchaseView, !_streamData.is_bought);
        _streamLikesRefresherView?.StartCountAsync(_streamData.id.ToString());
    }

    /// <summary>
    /// Change Status
    /// </summary>
    /// <param name="status"></param>
    public void ChangeHideBottomBarStatus(bool status) {
        Refresh();
    }

    private void OnEnable() {
        commentsToggle.onValueChanged.AddListener(ChangeHideBottomBarStatus);
        _purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        newDataAssigned = false;
        commentsToggle.onValueChanged.RemoveListener(ChangeHideBottomBarStatus);
        if (_purchaseManager != null) {
            _purchaseManager.OnPurchaseSuccessful -= Refresh;
        }
    }

    /// <summary>
    /// Close Prerecorded video window
    /// </summary>
    public void Close() {
        RecordARConstructor.OnActivated?.Invoke(false);
        ARenaConstructor.onDeactivate?.Invoke();
        PrerecordedVideoConstructor.OnDeactivated?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
    }
}
