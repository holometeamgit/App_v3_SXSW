using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;

public class RoomDeeplinkTester : MonoBehaviour
{
    [SerializeField] DeepLinkHandler DeepLinkHandler;
    [SerializeField] string linkRoomId;

    [ContextMenu("SendRoomId")]
    public void SendRoomId()
    {
        Debug.Log("RoomID " + linkRoomId);
        DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(linkRoomId);
    }
}
