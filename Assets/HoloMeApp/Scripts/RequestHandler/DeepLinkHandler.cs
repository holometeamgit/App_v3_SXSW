using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    private const string ROOM = "room";

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
        if (IsRoom(uri))
        {
            HelperFunctions.DevLog("GetRoomParameters");

            string roomId = GetRoom(uri);

            HelperFunctions.DevLog("roomId = " + roomId);
            StreamCallBacks.onRoomLinkReceived?.Invoke(roomId);
        }
    }

    private bool IsRoom(Uri uri) {
          return uri.LocalPath.Contains(ROOM);
    }

    private string GetRoom(Uri uri)
    {
        string localPath = uri.LocalPath;
        localPath = localPath.Substring(1, localPath.Length - 1);
        string[] split = localPath.Split('/');
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Contains(ROOM) && i < split.Length-1)
            {
                return split[i+1];
            }
        }
        return string.Empty;
    }
}