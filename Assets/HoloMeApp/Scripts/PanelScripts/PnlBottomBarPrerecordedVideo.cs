using System.Collections;
using System.Collections.Generic;
using Beem.Firebase.DynamicLink;
using Beem.UI;
using UnityEngine;

/// <summary>
/// Panel Bottom Bar for Prerecorded Video
/// </summary>
public class PnlBottomBarPrerecordedVideo : MonoBehaviour {

    [Header("Btn Likes")]
    [SerializeField]
    private UIBtnLikes _uiBtnLikes;

    private StreamJsonData.Data _streamData = default;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;
        _uiBtnLikes.SetStreamId(streamData.id);

        gameObject.SetActive(true);

    }

    /// <summary>
    /// Share prerecorded video
    /// </summary>
    public void Share() {
        if (!string.IsNullOrWhiteSpace(_streamData.id.ToString())) {
            StreamCallBacks.onGetStreamLink?.Invoke(_streamData.id.ToString());
        } else {
            DynamicLinksCallBacks.onShareAppLink?.Invoke();
        }
    }

    /// <summary>
    /// Open Comment
    /// </summary>
    public void OpenComment() {
        StreamCallBacks.onOpenComment?.Invoke((int)_streamData.id);
    }

}
