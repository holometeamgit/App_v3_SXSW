using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLinkHandler : MonoBehaviour
{
    private string roomId = "";

    private void Awake() {
        StreamCallBacks.onRoomLinkReceived += (id) => {
            roomId = id;
            StreamCallBacks.onOpenRoom?.Invoke();
        };

        StreamCallBacks.onCancelOpenRoom += () => PopRoomId();
    }

    public string PopRoomId() {
        string popString = roomId;
        HelperFunctions.DevLog("PopRoomId " + roomId);
        roomId = "";
        return popString;
    }

    public bool HasRoomId() {
        HelperFunctions.DevLog("HasRoomId " + !string.IsNullOrWhiteSpace(roomId));
        return !string.IsNullOrWhiteSpace(roomId);
    }
}
