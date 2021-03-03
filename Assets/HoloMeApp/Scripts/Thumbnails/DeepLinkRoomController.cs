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

    private void GetMyRoom() {
        webRequestHandler.GetRequest(GetMyRoomIdUrl(),
            (code, body) => MyRoomIdRecieved(body),
            (code, body) => HelperFunctions.DevLogError(code + " " + body),
            accountManager.GetAccessToken().access);
    }

    private void MyRoomIdRecieved(string body) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);
            //room?roomid=string
            string uri = serverURLAPIScriptableObject.FirebaseDynamicLinkAPI + "/" + roomJsonData.id;
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(serverURLAPIScriptableObject.FirebaseDynamicLinkAPI, uri);
        } catch { }
    }

    private void Awake() {
        StreamCallBacks.onGetMyRoomLink += GetMyRoom;
    }

    private string GetMyRoomIdUrl() {
        return serverURLAPIScriptableObject.ServerURLMediaAPI + videoUploader.GetRoom;
    }
}
