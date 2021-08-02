using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;

public class DeepLinkRoomController : MonoBehaviour {
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;
    [SerializeField] VideoUploader videoUploader;
    [SerializeField] AccountManager accountManager;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    private void GetMyRoom(string source) {
        HelperFunctions.DevLog("GetMyRoom");
        webRequestHandler.GetRequest(GetMyRoomIdUrl(),
            (code, body) => MyRoomIdRecieved(body, source),
            (code, body) => HelperFunctions.DevLogError(code + " " + body),
            accountManager.GetAccessToken().access);
    }

    private void MyRoomIdRecieved(string body, string source) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);
            //room?roomid=string
            HelperFunctions.DevLog("MyRoomIdRecieved = " + body);
            DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.Room, roomJsonData.id, serverURLAPIScriptableObject.Url);
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters, source);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void GetRoomLink(string id, string source) {
        DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.Room, id, serverURLAPIScriptableObject.Url);
        DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters, source);
    }

    private void Awake() {
        StreamCallBacks.onGetMyRoomLink += GetMyRoom;
        StreamCallBacks.onGetRoomLink += GetRoomLink;
    }

    private void OnDestroy() {
        StreamCallBacks.onGetMyRoomLink -= GetMyRoom;
        StreamCallBacks.onGetRoomLink -= GetRoomLink;
    }

    private string GetMyRoomIdUrl() {
        return serverURLAPIScriptableObject.ServerURLMediaAPI + videoUploader.GetRoom;
    }
}
