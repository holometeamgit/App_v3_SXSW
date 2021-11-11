using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    public void OnDynamicLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLog("Dynamic link: " + uriStr);
        GetContentsParameters(uri);
    }

    private void Awake() {
        DynamicLinksCallBacks.onReceivedDeepLink += OnDynamicLinkActivated;
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainFolder(uri, DynamicLinkParameters.Folder.stream.ToString())) {

            HelperFunctions.DevLog("GetStreamParameters");

            string streamId = GetFolderId(uri, DynamicLinkParameters.Folder.stream.ToString());

            HelperFunctions.DevLog("streamId = " + streamId);
            StreamCallBacks.onStreamLinkReceived?.Invoke(streamId);
        } else if (ContainFolder(uri, DynamicLinkParameters.Folder.room.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetFolderId(uri, DynamicLinkParameters.Folder.room.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);
        } else if (ContainParameter(uri, DynamicLinkParameters.Folder.username.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameterId(uri, DynamicLinkParameters.Folder.username.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);
        }
    }

    private bool ContainFolder(Uri uri, string parameter) {
        return uri.LocalPath.Contains(parameter);
    }

    private string GetFolderId(Uri uri, string parameter) {
        string localPath = uri.LocalPath;
        localPath = localPath.Substring(1, localPath.Length - 1);
        string[] split = localPath.Split('/');
        for (int i = 0; i < split.Length; i++) {
            if (split[i].Contains(parameter) && i < split.Length - 1) {
                return split[i + 1];
            }
        }
        return string.Empty;
    }

    private bool ContainParameter(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter) != null;
    }

    private string GetParameterId(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }
}