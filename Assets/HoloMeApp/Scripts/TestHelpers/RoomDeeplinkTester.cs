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

    [ContextMenu("Test by script")]
    private void Test() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "ivklim21";
        roomJsonData.user = "ivklim21";
        roomJsonData.status = "live_room";

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(1000).ContinueWith(task => {
            StreamCallBacks.onRoomDataReceived?.Invoke(roomJsonData);
        }, taskScheduler);

        Task.Delay(5000).ContinueWith(task => {
            StreamCallBacks.onOpenRoom?.Invoke();
            StreamCallBacks.onRoomClosed?.Invoke();
        }, taskScheduler);
    }
}
