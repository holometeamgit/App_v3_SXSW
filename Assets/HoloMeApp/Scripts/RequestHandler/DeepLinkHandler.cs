using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {

    public enum Query {
        username,
        message,
        live,
        prerecorded
    }

    public enum Folder {
        room,
        message,
        live,
        prerecorded
    }

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
        if (ContainParameter(uri, Query.username.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameterId(uri, Query.username.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onReceiveRoomLink?.Invoke(userName);
        } else if (ContainParameter(uri, Query.message.ToString())) {

            HelperFunctions.DevLog("GetMessageParameters");

            string messageId = GetParameterId(uri, Query.message.ToString());

            HelperFunctions.DevLog("messageId = " + messageId);
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainParameter(uri, Query.live.ToString())) {

            HelperFunctions.DevLog("GetLiveParameters");

            string username = GetParameterId(uri, Query.live.ToString());

            HelperFunctions.DevLog("username = " + username);
            StreamCallBacks.onReceiveStreamLink?.Invoke(username);
        } else if (ContainParameter(uri, Query.prerecorded.ToString())) {

            HelperFunctions.DevLog("GetPrerecordedParameters");

            string slug = GetParameterId(uri, Query.prerecorded.ToString());

            HelperFunctions.DevLog("slug = " + slug);
            StreamCallBacks.onReceivePrerecordedLink?.Invoke(slug);
        } else if (ContainFolder(uri, Folder.room.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetFolderId(uri, Folder.room.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onReceiveRoomLink?.Invoke(userName);
        } else if (ContainFolder(uri, Folder.message.ToString())) {

            HelperFunctions.DevLog("GetMessageParameters");

            string messageId = GetFolderId(uri, Folder.message.ToString());

            HelperFunctions.DevLog("messageId = " + messageId);
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainFolder(uri, Folder.live.ToString())) {

            HelperFunctions.DevLog("GetLiveParameters");

            string username = GetFolderId(uri, Folder.live.ToString());

            HelperFunctions.DevLog("username = " + username);
            StreamCallBacks.onReceiveStreamLink?.Invoke(username);
        } else if (ContainFolder(uri, Folder.prerecorded.ToString())) {

            HelperFunctions.DevLog("GetPrerecordedParameters");

            string slug = GetFolderId(uri, Folder.prerecorded.ToString());

            HelperFunctions.DevLog("slug = " + slug);
            StreamCallBacks.onReceivePrerecordedLink?.Invoke(slug);
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
                return split[split.Length - 1];
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