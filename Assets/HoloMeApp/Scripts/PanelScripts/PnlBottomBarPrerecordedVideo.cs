using Beem.Firebase.DynamicLink;
using Beem.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel Bottom Bar for Prerecorded Video
/// </summary>
public class PnlBottomBarPrerecordedVideo : MonoBehaviour {

    [Header("Btn Likes")]
    [SerializeField]
    private UIBtnLikes _uiBtnLikes;

    [Header("Comment toggle")]
    [SerializeField]
    private Toggle _uiToggleComments;

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

    private void Awake() {
        StreamCallBacks.onCommentsClosed += ForceCommentsToggleOff;
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
    /// Toggle comments
    /// </summary>
    public void ToggleComments(bool enable) {
        if (enable) {
            StreamCallBacks.onOpenComment?.Invoke((int)_streamData.id);
        } else {
            StreamCallBacks.onCloseComments?.Invoke();
        }
    }

    private void ForceCommentsToggleOff() {
        _uiToggleComments.isOn = false;
    }

    private void OnDestroy() {
        StreamCallBacks.onCommentsClosed -= ForceCommentsToggleOff;
    }

}
