using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using System.Threading;
using System.Threading.Tasks;

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

    [ContextMenu("send room data")]
    private void TestRoomData() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "ivklim21";
        roomJsonData.user = "ivklim21";
        roomJsonData.status = "live_room";

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(500).ContinueWith(task => {
            StreamCallBacks.onRoomDataReceived?.Invoke(roomJsonData);
        }, taskScheduler);
    }

    [ContextMenu("send that room was close ")]
    private void Test() {
        StreamCallBacks.onRoomClosed?.Invoke();
    }
}
