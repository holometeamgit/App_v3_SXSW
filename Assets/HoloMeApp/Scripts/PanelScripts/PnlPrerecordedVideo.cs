using System.Collections;
using System.Collections.Generic;
using Beem.Firebase.DynamicLink;
using Beem.UI;
using UnityEngine;

/// <summary>
/// Panel for Prerecorded Video
/// </summary>
public class PnlPrerecordedVideo : MonoBehaviour {

    [Header("View for video")]
    [SerializeField]
    private GameObject _videoView;

    [Header("Purchase for video")]
    [SerializeField]
    private GameObject _purchaseView;


    [Header("Bottom Bar")]
    [SerializeField]
    private BottomBar _bottomBar;

    private StreamJsonData.Data _streamData = default;

    private bool newDataAssigned = false;

    private bool isPinned = false;

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
        gameObject.SetActive(true);
        hologramHandler.SetOnPlacementUIHelperFinished(Refresh);
        _bottomBar.Init(streamData);
    }

    private void Deactivate() {
        _videoView.SetActive(false);
        _purchaseView.SetActive(false);
        _bottomBar.gameObject.SetActive(false);
    }

    private void Refresh() {
        if (!newDataAssigned) {
            return;
        }
        isPinned = true;
        _videoView.SetActive(_streamData.IsStarted && _streamData.is_bought);
        _purchaseView.SetActive(!_streamData.is_bought);
        _bottomBar.gameObject.SetActive(true);
    }

    private void OnEnable() {
        purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        newDataAssigned = false;
        Deactivate();
        if (purchaseManager != null) {
            purchaseManager.OnPurchaseSuccessful -= Refresh;
        }
    }
}
