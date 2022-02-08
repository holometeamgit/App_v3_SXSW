using System;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;
using Zenject;

/// <summary>
/// Deep Link Controller for RoomData
/// </summary>
public class DeepLinkRoomController : MonoBehaviour {
    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;
    [SerializeField]
    private VideoUploader _videoUploader;

    private WebRequestHandler _webRequestHandler;


    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    private ShareLinkController _shareController = new ShareLinkController();

    private const string TITLE = "You have been invited to {0}'s Room";
    private const string DESCRIPTION = "Click the link below to join {0}'s Room";

    private void GetRoomByUserName(string username, Action<long, string> onSuccess, Action<long, string> onFailed) {
        HelperFunctions.DevLog("Get Room By UserName " + username);
        _webRequestHandler.Get(GetRoomUsernameUrl(username),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(code, body); },
            needHeaderAccessToken: false);
    }

    private void RoomReceived(string body, Action<RoomJsonData> onReceived) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);

            HelperFunctions.DevLog("Room Recieved = " + body);

            onReceived?.Invoke(roomJsonData);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void OnOpen(string username) {
        GetRoomByUserName(username,
            (code, body) => Open(body),
            (code, body) => {
                StreamCallBacks.onUserDoesntExist(code); HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void Open(string body) {
        RoomReceived(body,
            (data) => {
                StreamCallBacks.onRoomDataReceived?.Invoke(data);
            });
    }

    private void OnShare(string username) {
        GetRoomByUserName(username,
            (code, body) => Share(body),
            (code, body) => {
                HelperFunctions.DevLogError(code + " " + body);
            });
    }

    private void Share(string body) {
        RoomReceived(body,
            (data) => {
                _shareController.ShareSocialLink(new Uri(data.share_link), SocialParameters(data.user));
            });
    }

    private void Awake() {
        StreamCallBacks.onShareRoomLink += OnShare;
        StreamCallBacks.onReceiveRoomLink += OnOpen;
    }

    private void OnDestroy() {
        StreamCallBacks.onShareRoomLink -= OnShare;
        StreamCallBacks.onReceiveRoomLink -= OnOpen;
    }

    private string GetRoomUsernameUrl(string username) {
        return _serverURLAPIScriptableObject.ServerURLMediaAPI + _videoUploader.GetRoomByUserName.Replace("{username}", username.ToString());
    }

    public SocialMetaTagParameters SocialParameters(string source) {
        SocialMetaTagParameters socialMetaTagParameters = new SocialMetaTagParameters() {
            Title = string.Format(TITLE, source),
            Description = string.Format(DESCRIPTION, source),
            ImageUrl = new Uri(_serverURLAPIScriptableObject.LogoLink)
        };
        return socialMetaTagParameters;
    }
}
