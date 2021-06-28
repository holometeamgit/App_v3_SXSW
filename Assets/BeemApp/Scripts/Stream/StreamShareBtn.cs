using Beem.Firebase.DynamicLink;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StreamShareBtn : MonoBehaviour, IPointerDownHandler {

    private string _streamID = default;
    private bool _isRoom = default;

    /// <summary>
    /// Initialise
    /// </summary>
    /// <param name="streamID"></param>
    /// <param name="isRoom"></param>
    public void Init(string streamID, bool isRoom) {
        _streamID = streamID;
        _isRoom = isRoom;
    }

    public void OnPointerDown(PointerEventData eventData) {
        Share();
    }

    /// <summary>
    /// Share
    /// </summary>
    private void Share() {
        if (!string.IsNullOrWhiteSpace(_streamID)) {
            if (_isRoom) {
                StreamCallBacks.onGetStreamLink?.Invoke(_streamID);
            } else {
                StreamCallBacks.onGetRoomLink?.Invoke(_streamID);
            }
        } else {
            DynamicLinksCallBacks.onShareAppLink?.Invoke();
        }
    }
}
