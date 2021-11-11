using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using System.Threading;
using System.Threading.Tasks;

public class RoomDeeplinkTester : MonoBehaviour {
    [SerializeField] string linkRoomId;

    [ContextMenu("SendRoomId")]
    public void SendRoomId() {
        Debug.Log("RoomID " + linkRoomId);
        DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(linkRoomId);
    }

    [ContextMenu("Get onUsernameLinkReceived ")]
    private void TestOnUsernameLinkReceived() {
        StreamCallBacks.onUsernameLinkReceived?.Invoke("ivklim21");
    }

    [ContextMenu("Get test event online")]
    private void TestOnlineRoomData() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "ivklim21";
        roomJsonData.user = "ivklim21";
        roomJsonData.status = "live_room";

        StreamCallBacks.onRoomDataReceived?.Invoke(roomJsonData);
    }

    [ContextMenu("Get test event offline")]
    private void TestOflineRoomData() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "ivklim21";
        roomJsonData.user = "ivklim21";
        roomJsonData.status = "stop";

        StreamCallBacks.onRoomDataReceived?.Invoke(roomJsonData);
    }

    //see popup -> click open -> wait 5 sec -> close stream -> you will see close popup
    [ContextMenu("send that room was close ")]
    private void TestClosedRoomEvent() {
        TestOnlineRoomData();

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(5000).ContinueWith(task => {
            StreamCallBacks.onRoomClosed?.Invoke();
        }, taskScheduler);
    }
}
