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

    private bool isBought {
        get {
            if (_purchaseManager == null) {
                _purchaseManager = FindObjectOfType<PurchaseManager>();
            }

            return _purchaseManager.IsBought();
        }
    }


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;
        _teaserView.SetActive(_streamData.HasTeaser && isBought);
        _videoView.SetActive(!_streamData.HasTeaser);
        gameObject.SetActive(true);
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
