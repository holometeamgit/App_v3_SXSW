using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;
using System.Threading;
using System.Threading.Tasks;

public class DeeplinkTester : MonoBehaviour {
    [SerializeField] string linkRoomId;

    [ContextMenu("CallLink")]
    public void CallLink() {
        DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(linkRoomId);
    }

    [ContextMenu("Get onUsernameLinkReceived ")]
    private void TestOnUsernameLinkReceived() {
        StreamCallBacks.onReceiveRoomLink?.Invoke("ivklim21");
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
}
