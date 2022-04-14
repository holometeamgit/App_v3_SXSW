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

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _getRoomController = new GetRoomController(_videoUploader, webRequestHandler);
    }

    private void OnOpen(string username) {
        _getRoomController.GetRoomByUsername(username, (data) => {
            StreamCallBacks.onRoomDataReceived?.Invoke(data);
            DeepLinkStreamConstructor.OnShow?.Invoke(data);
        }, DeepLinkStreamConstructor.OnShowError);
    }

    private void Awake() {
        StreamCallBacks.onReceiveRoomLink += OnOpen;
    }

    private void OnDestroy() {
        StreamCallBacks.onReceiveRoomLink -= OnOpen;
    }

}
