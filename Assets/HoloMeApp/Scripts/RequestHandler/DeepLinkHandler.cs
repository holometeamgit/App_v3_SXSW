using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    [SerializeField]
    private ServerURLAPIScriptableObject serverURLAPIScriptableObject;

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
        if (IsFolder(uri, serverURLAPIScriptableObject.Room)){
            HelperFunctions.DevLog("GetRoomParameters");

            string roomId = GetId(uri, serverURLAPIScriptableObject.Room);

            HelperFunctions.DevLog("roomId = " + roomId);
            StreamCallBacks.onRoomLinkReceived?.Invoke(roomId);
        }
        else if (IsFolder(uri, serverURLAPIScriptableObject.Stream)){
            HelperFunctions.DevLog("GetStreamParameters");

            string streamId = GetId(uri, serverURLAPIScriptableObject.Stream);

            HelperFunctions.DevLog("streamId = " + streamId);
            StreamCallBacks.onStreamLinkReceived?.Invoke(streamId);
        }
        else if (IsFolder(uri, serverURLAPIScriptableObject.NotificationAccess)) { 
            PermissionController permissionController = FindObjectOfType<PermissionController>();
            permissionController.CheckPushNotifications();
        }
    }

    private bool IsFolder(Uri uri, string folder) {
          return uri.LocalPath.Contains(folder);
    }

    private string GetId(Uri uri, string folder)
    {
        string localPath = uri.LocalPath;
        localPath = localPath.Substring(1, localPath.Length - 1);
        string[] split = localPath.Split('/');
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Contains(folder) && i < split.Length-1)
            {
                return split[i+1];
            }
        }
        return string.Empty;
    }
}