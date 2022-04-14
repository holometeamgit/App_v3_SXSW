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
    private VideoUploader _videoUploader;

    private GetRoomController _getRoomController;

    private const string TITLE = "You have been invited to {0}'s Room";
    private const string DESCRIPTION = "Click the link below to join {0}'s Room";

    private ShareLinkController _shareController = new ShareLinkController();

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getRoomController = new GetRoomController(_videoUploader, webRequestHandler);
    }

    private void OnOpen(string username) {
        _getRoomController.GetRoomByUsername(username, (data) => {
            StreamCallBacks.onRoomDataReceived?.Invoke(data);
            DeepLinkRoomConstructor.OnShow?.Invoke(data);
        }, DeepLinkRoomConstructor.OnShowError);
    }

    private void OnShare(string username) {
        _getRoomController.GetRoomByUsername(username, (data) => {
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

}
