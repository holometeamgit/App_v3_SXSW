using Beem.Firebase.DynamicLink;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShareStreamBtn : MonoBehaviour, IStreamData, IPointerDownHandler {

    private StreamJsonData.Data _streamData = default;

    public void Init(StreamJsonData.Data streamData) {
        _streamData = streamData;
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

    public void OnPointerDown(PointerEventData eventData) {
        Share();
    }
}
