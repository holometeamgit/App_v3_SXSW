using System.Collections;
using System.Collections.Generic;
using Beem.Firebase.DynamicLink;
using UnityEngine;

/// <summary>
/// Panel for Prerecorded Video
/// </summary>
public class PnlPrerecordedVideo : MonoBehaviour {

    [Header("View for teaser")]
    [SerializeField]
    private GameObject _teaserView;

    [Header("View for video")]
    [SerializeField]
    private GameObject _videoView;

    private StreamJsonData.Data _streamData = default;

    private PurchaseManager _purchaseManager;

    private PurchaseManager purchaseManager {
        get {
            if (_purchaseManager == null) {
                _purchaseManager = FindObjectOfType<PurchaseManager>();
            }

            return _purchaseManager;
        }
    }

    private HologramHandler _hologramHandler;

    private HologramHandler hologramHandler {
        get {
            if (_hologramHandler == null) {
                _hologramHandler = FindObjectOfType<HologramHandler>();
            }

            return _hologramHandler;
        }
    }

    private void OnEnable() {
        purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        purchaseManager.OnPurchaseSuccessful -= Refresh;
    }


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;

        _teaserView.SetActive(false);
        _videoView.SetActive(false);

        gameObject.SetActive(true);

        hologramHandler.SetOnPlacementUIHelperFinished(Refresh);
    }

    private void Refresh() {
        _teaserView.SetActive(_streamData.HasTeaser && purchaseManager.IsBought());
        _videoView.SetActive(!_streamData.HasTeaser);
    }

    /// <summary>
    /// Share prerecorded video
    /// </summary>
    public void Share() {
        if (!string.IsNullOrWhiteSpace(_streamData.id.ToString()))
            StreamCallBacks.onGetStreamLink?.Invoke(_streamData.id.ToString());
        else
            DynamicLinksCallBacks.onShareAppLink?.Invoke();
    }

    /// <summary>
    /// Open Comment
    /// </summary>
    public void OpenComment() {
        StreamCallBacks.onOpenComment?.Invoke((int)_streamData.id);
    }
}
