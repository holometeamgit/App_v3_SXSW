using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentLinkHandler : MonoBehaviour
{
    private string contentId = "";
    private ContentLinkHandlerType contentLinkHandlerType;

    public string ContentId {
        get {
            HelperFunctions.DevLog("contentID " + contentId);
            return contentId;
        }
    }

    private void Awake() {
        StreamCallBacks.onRoomLinkReceived += OnRoomLinkReceive;
        StreamCallBacks.onStreamLinkReceived += OnStreamLinkReceive;
        StreamCallBacks.onCancelOpenContent += OnClear;
    }

    public string PopContentId() {
        string popString = ContentId;
        OnClear();
        return popString;
    }

    public bool HasContentId(ContentLinkHandlerType type) {
        HelperFunctions.DevLog("HasRoomId type = " + contentLinkHandlerType + " id = " + !string.IsNullOrWhiteSpace(contentId));
        return contentLinkHandlerType == type && !string.IsNullOrWhiteSpace(contentId);
    }

    private void OnClear() {
        contentId = "";
        contentLinkHandlerType = ContentLinkHandlerType.None;
    }

    private void OnRoomLinkReceive(string id) {
        contentId = id;
        contentLinkHandlerType = ContentLinkHandlerType.Room;
        StreamCallBacks.onOpenRoom?.Invoke();
    }

    private void OnStreamLinkReceive(string id) {
        HelperFunctions.DevLog("onStreamLinkReceived " + id);
        contentId = id;
        contentLinkHandlerType = ContentLinkHandlerType.Stream;
        StreamCallBacks.onOpenStream?.Invoke();
    }

    private void OnDestroy() {
        StreamCallBacks.onRoomLinkReceived -= OnRoomLinkReceive;
        StreamCallBacks.onStreamLinkReceived -= OnStreamLinkReceive;
        StreamCallBacks.onCancelOpenContent -= OnClear;
    }

}
