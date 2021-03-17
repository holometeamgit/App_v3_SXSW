using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {
    //public static DeepLinkHandler Instance { get; private set; }

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    public void OnDynamicLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLog("Dynamic link: " + uriStr);
        GetRoomParameters(uri);
    }

    private void Awake() {
        DynamicLinksCallBacks.onReceivedDeepLink += OnDynamicLinkActivated;
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
    }

    private void GetRoomParameters(Uri uri) {
        HelperFunctions.DevLog("GetRoomParameters");

        string roomId = uri.LocalPath.Replace("/", "");

        HelperFunctions.DevLog("roomId = " + roomId);
        StreamCallBacks.onRoomLinkReceived?.Invoke(roomId);
    }
}