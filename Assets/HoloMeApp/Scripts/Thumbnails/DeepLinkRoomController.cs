using System;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;

public class DeepLinkRoomController : MonoBehaviour {
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;
    [SerializeField] VideoUploader videoUploader;
    [SerializeField] AccountManager accountManager;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    private const string TITLE = "You have been invited to {0}'s Room";
    private const string DESCRIPTION = "Click the link below to join {0}'s Room";

    private void GetMyRoom() {
        HelperFunctions.DevLog("GetMyRoom");
        webRequestHandler.GetRequest(GetMyRoomIdUrl(),
            (code, body) => MyRoomIdRecieved(body),
            (code, body) => HelperFunctions.DevLogError(code + " " + body),
            accountManager.GetAccessToken().access);
    }

    private void MyRoomIdRecieved(string body) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);
            //room?roomid=string
            HelperFunctions.DevLog("MyRoomIdRecieved = " + body);
            DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.Room, roomJsonData.id, serverURLAPIScriptableObject.Url, SocialParameters(roomJsonData.user));
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters, roomJsonData.user);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void GetRoomLink(string id, string source) {
        DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.Room, id, serverURLAPIScriptableObject.Url, SocialParameters(source));
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

    public SocialMetaTagParameters SocialParameters(string source) {
        SocialMetaTagParameters socialMetaTagParameters = new SocialMetaTagParameters() {
            Title = string.Format(TITLE, source),
            Description = string.Format(DESCRIPTION, source),
            ImageUrl = new Uri(serverURLAPIScriptableObject.LogoLink)
        };
        return socialMetaTagParameters;
    }
}
