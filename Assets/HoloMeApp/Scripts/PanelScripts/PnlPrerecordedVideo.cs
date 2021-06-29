using System.Collections;
using System.Collections.Generic;
using Beem.Firebase.DynamicLink;
using Beem.UI;
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

    [Header("Bottom Bar")]
    [SerializeField]
    private PnlBottomBarPrerecordedVideo _bottomBar;

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

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;
#if !UNITY_EDITOR
        Activate(false);
#else
        Refresh();
#endif
        gameObject.SetActive(!(_streamData.HasTeaser && !purchaseManager.IsBought()));
        hologramHandler.SetOnPlacementUIHelperFinished(Refresh);
        _bottomBar.Init(streamData);
    }

    private void Activate(bool value) {
        _teaserView.SetActive(value);
        _videoView.SetActive(value);
        _bottomBar.gameObject.SetActive(value);
    }

    private void Refresh() {
        HelperFunctions.DevLog("_streamData.HasTeaser = " + _streamData.HasTeaser);
        HelperFunctions.DevLog("purchaseManager.IsBought = " + purchaseManager.IsBought());
        _teaserView.SetActive(_streamData.HasTeaser && !purchaseManager.IsBought());
        _videoView.SetActive(!(_streamData.HasTeaser && !purchaseManager.IsBought()));
        _bottomBar.gameObject.SetActive(!(_streamData.HasTeaser && !purchaseManager.IsBought()));
    }

    private void OnEnable() {
        purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        Activate(false);
        if (purchaseManager != null) {
            purchaseManager.OnPurchaseSuccessful -= Refresh;
        }
    }
}
