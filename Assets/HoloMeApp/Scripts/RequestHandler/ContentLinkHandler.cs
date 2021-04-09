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
        StreamCallBacks.onRoomLinkReceived += (id) => {
            contentId = id;
            contentLinkHandlerType = ContentLinkHandlerType.Room;
            StreamCallBacks.onOpenRoom?.Invoke();
        };

        StreamCallBacks.onStreamLinkReceived += (id) => {
            contentId = id;
            contentLinkHandlerType = ContentLinkHandlerType.Room;
            StreamCallBacks.onOpenStream?.Invoke();
        };

        StreamCallBacks.onCancelOpenContent += () => PopContentId();
    }

    public string PopContentId() {
        string popString = ContentId;
        contentId = "";
        contentLinkHandlerType = ContentLinkHandlerType.None;
        return popString;
    }

    public bool HasContentId(ContentLinkHandlerType type) {
        HelperFunctions.DevLog("HasRoomId type = " + contentLinkHandlerType + " id = " + !string.IsNullOrWhiteSpace(contentId));
        return contentLinkHandlerType == type && !string.IsNullOrWhiteSpace(contentId);
    }
}
