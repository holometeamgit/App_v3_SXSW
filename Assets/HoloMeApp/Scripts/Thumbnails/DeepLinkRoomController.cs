using System;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using UnityEngine;

/// <summary>
/// Deep Link Controller for RoomData
/// </summary>
public class DeepLinkRoomController : MonoBehaviour {
    [SerializeField]
    private WebRequestHandler _webRequestHandler;
    [SerializeField]
    private ServerURLAPIScriptableObject _serverURLAPIScriptableObject;
    [SerializeField]
    private VideoUploader _videoUploader;

    private const string TITLE = "You have been invited to {0}'s Room";
    private const string DESCRIPTION = "Click the link below to join {0}'s Room";

    private ShareLinkController _shareController = new ShareLinkController();

    private void GetRoomByUserName(string username, Action<long, string> onSuccess, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetRoomUsernameUrl(username),
            (code, body) => { onSuccess?.Invoke(code, body); },
            (code, body) => { onFailed?.Invoke(new WebRequestError(code, body)); },
            needHeaderAccessToken: false);
    }

    private void RoomReceived(string body, Action<RoomJsonData> OnSuccess, Action<WebRequestError> onFailed = null) {
        try {
            RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(body);

            HelperFunctions.DevLog("Room Recieved = " + body);

            OnSuccess?.Invoke(roomJsonData);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnOpen(string username) {
        GetRoomByUserName(username,
            (code, body) => Open(body),
            DeepLinkRoomConstructor.OnShowError);
    }

    private void Open(string body) {
        RoomReceived(body,
            (data) => {
                StreamCallBacks.onRoomDataReceived?.Invoke(data);
                DeepLinkRoomConstructor.OnShow?.Invoke(data);
            }, DeepLinkRoomConstructor.OnShowError);
    }

    private void OnShare(string username) {
        GetRoomByUserName(username,
            (code, body) => Share(body));
    }

    private void Share(string body) {
        RoomReceived(body,
            (data) => {
                string title = string.Format(TITLE, data.user);
                string description = string.Format(DESCRIPTION, data.user);
                string msg = title + "\n" + description + "\n" + data.share_link;
                _shareController.ShareLink(msg);
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
}
