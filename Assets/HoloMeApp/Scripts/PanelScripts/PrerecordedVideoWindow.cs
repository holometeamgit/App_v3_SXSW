using Beem.Permissions;
using Beem.UI;
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
    private GameObject _placementView;

    [Header("View for video")]
    [SerializeField]
    private GameObject _videoView;

    [Header("Purchase for video")]
    [SerializeField]
    private GameObject _purchaseView;

    [Header("Comments Toggle")]
    [SerializeField]
    private Toggle commentsToggle;

    [Header("Purchase manager")]
    [SerializeField]
    private PurchaseManager _purchaseManager;

    [Header("Hologram manager")]
    [SerializeField]
    private HologramHandler _hologramHandler;

    [Header("Permission Controller")]
    [SerializeField]
    private PermissionController _permissionController;

    private StreamJsonData.Data _streamData = default;

    private bool newDataAssigned = false;

    private bool isPinned = false;


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        newDataAssigned = true;
        _streamData = streamData;
        if (isPinned) {
            OnPlacementCompleted();
        } else {
#if !UNITY_EDITOR
            _placementView.SetActive(false);
#else
            OnPlacementCompleted();
#endif
        }

        gameObject.SetActive(true);
        Refresh();
        _permissionController.CheckCameraPermission();
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
        _placementView.SetActive(true);
        RecordARConstructor.OnActivated?.Invoke(_streamData.IsStarted && _streamData.is_bought);
    }

    private void Refresh() {
        if (!newDataAssigned) {
            return;
        }

        _videoView.SetActive(_streamData.IsStarted && _streamData.is_bought && !commentsToggle.isOn);
        _purchaseView.SetActive(!_streamData.is_bought && !commentsToggle.isOn);
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
        MenuConstructor.OnActivated?.Invoke(true);
        HomeScreenConstructor.OnActivated?.Invoke(true);
        PrerecordedVideoConstructor.OnDeactivated?.Invoke();
    }
}
