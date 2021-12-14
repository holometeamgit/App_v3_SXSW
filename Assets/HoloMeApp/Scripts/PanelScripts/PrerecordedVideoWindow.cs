using UnityEngine;

/// <summary>
/// Panel for Prerecorded Video
/// </summary>
public class PrerecordedVideoWindow : MonoBehaviour {

    [Header("View for video")]
    [SerializeField]
    private GameObject _videoView;

    [Header("Purchase for video")]
    [SerializeField]
    private GameObject _purchaseView;

    [Header("Purchase manager")]
    [SerializeField]
    private PurchaseManager _purchaseManager;

    [Header("Hologram manager")]
    [SerializeField]
    private HologramHandler _hologramHandler;

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
            Refresh();
        } else {
#if !UNITY_EDITOR
            Deactivate();
#else
            Refresh();
#endif
        }
        _hologramHandler.SetOnPlacementUIHelperFinished(Refresh);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
        _videoView.SetActive(false);
        _purchaseView.SetActive(false);
    }

    private void Refresh() {
        if (!newDataAssigned) {
            return;
        }
        isPinned = true;
        gameObject.SetActive(true);
        _videoView.SetActive(_streamData.IsStarted && _streamData.is_bought);
        _purchaseView.SetActive(!_streamData.is_bought);
    }

    private void OnEnable() {
        _purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        newDataAssigned = false;
        if (_purchaseManager != null) {
            _purchaseManager.OnPurchaseSuccessful -= Refresh;
        }
    }

    /// <summary>
    /// Close Prerecorded video window
    /// </summary>
    public void Close() {
        MenuConstructor.OnActivated?.Invoke(true);
        HomeScreenConstructor.OnActivated?.Invoke(true);
        PrerecordedVideoConstructor.OnDeactivated?.Invoke();
    }
}
