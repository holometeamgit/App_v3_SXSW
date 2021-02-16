using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDeeplinkTester : MonoBehaviour
{
    [SerializeField] DeepLinkHandler DeepLinkHandler;
    [SerializeField] string linkRoomId;

    [ContextMenu("SendRoomId")]
    public void SendRoomId()
    {
        Debug.Log("RoomID " + linkRoomId);
        DeepLinkHandler.OnDeepLinkActivated("beemholomedl://room?roomid=" + linkRoomId);
    }
}
