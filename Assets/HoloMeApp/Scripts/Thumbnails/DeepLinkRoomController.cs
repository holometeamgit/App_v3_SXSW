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

    private void GetRoomByUserName(string username) {
        HelperFunctions.DevLog("Get Room By UserName");
        webRequestHandler.GetRequest(GetRoomUsernameUrl(username),
            (code, body) => RoomReceived(body),
            (code, body) => HelperFunctions.DevLogError(code + " " + body),
            accountManager.GetAccessToken().access);
    }

    private void RoomReceived(string body) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);

            HelperFunctions.DevLog("Room Recieved = " + body);

            StreamCallBacks.onRoomLinkReceived?.Invoke(roomJsonData.id);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void GetRoomLink(string source) {
        Uri uri = new Uri(serverURLAPIScriptableObject.FirebaseDynamicLink + serverURLAPIScriptableObject.FirebaseRoom + source);
        DynamicLinksCallBacks.onGetShortLink?.Invoke(uri, SocialParameters(source));
    }

    private void Awake() {
        StreamCallBacks.onGetRoomLink += GetRoomLink;
        StreamCallBacks.onUsernameLinkReceived += GetRoomByUserName;
    }

    private void OnDestroy() {
        StreamCallBacks.onGetRoomLink -= GetRoomLink;
        StreamCallBacks.onUsernameLinkReceived -= GetRoomByUserName;
    }

    private string GetRoomUsernameUrl(string username) {
        return serverURLAPIScriptableObject.ServerURLMediaAPI + videoUploader.GetRoomByUserName + username;
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
