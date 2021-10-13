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
        if (ContainParameter(uri, DynamicLinkParameters.Parameter.slug.ToString())) {
            HelperFunctions.DevLog("GetStreamParameters");

            string streamId = GetParameterId(uri, DynamicLinkParameters.Parameter.slug.ToString());

            HelperFunctions.DevLog("streamId = " + streamId);
            StreamCallBacks.onStreamLinkReceived?.Invoke(streamId);
        } else if (ContainFolder(uri, DynamicLinkParameters.Folder.stream.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetFolderId(uri, DynamicLinkParameters.Folder.stream.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);
        } else if (ContainParameter(uri, DynamicLinkParameters.Parameter.username.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameterId(uri, DynamicLinkParameters.Parameter.username.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);
        } else if (ContainFolder(uri, DynamicLinkParameters.Folder.room.ToString())) {
            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetFolderId(uri, DynamicLinkParameters.Folder.room.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);

        }
    }

    private bool ContainParameter(Uri uri, string parameter) {
        return !string.IsNullOrEmpty(HttpUtility.ParseQueryString(uri.Query).Get(parameter));
    }

    private string GetParameterId(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }

    private bool ContainFolder(Uri uri, string folder) {
        return uri.LocalPath.Contains(folder);
    }

    private string GetFolderId(Uri uri, string folder) {
        string localPath = uri.LocalPath;
        localPath = localPath.Substring(1, localPath.Length - 1);
        string[] split = localPath.Split('/');
        for (int i = 0; i < split.Length; i++) {
            if (split[i].Contains(folder) && i < split.Length - 1) {
                return split[i + 1];
            }
        }
        return string.Empty;
    }
}