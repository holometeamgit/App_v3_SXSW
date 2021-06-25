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

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;
#if !UNITY_EDITOR
        _teaserView.SetActive(false);
        _videoView.SetActive(false);
#else
        Refresh();
#endif

        gameObject.SetActive(!(_streamData.HasTeaser && !purchaseManager.IsBought()));
    }

    private void Refresh() {
        _teaserView.SetActive(_streamData.HasTeaser && !purchaseManager.IsBought());
        _videoView.SetActive(!(_streamData.HasTeaser && !purchaseManager.IsBought()));
    }

    private void OnEnable() {
        purchaseManager.OnPurchaseSuccessful += Refresh;
    }

    private void OnDisable() {
        purchaseManager.OnPurchaseSuccessful -= Refresh;
    }
}
