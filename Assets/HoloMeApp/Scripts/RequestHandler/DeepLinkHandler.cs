using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {
    public enum Params {
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
        Application.deepLinkActivated += OnDynamicLinkActivated;
        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            // Cold start and Application.absoluteURL not null so process Deep Link.
            OnDynamicLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
        Application.deepLinkActivated -= OnDynamicLinkActivated;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainParam(uri, Params.room.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParam(uri, Params.room.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onReceiveRoomLink?.Invoke(userName);
        } else if (ContainParam(uri, Params.message.ToString())) {

            HelperFunctions.DevLog("GetMessageParameters");

            string messageId = GetParam(uri, Params.message.ToString());

            HelperFunctions.DevLog("messageId = " + messageId);
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainParam(uri, Params.live.ToString())) {

            HelperFunctions.DevLog("GetLiveParameters");

            string username = GetParam(uri, Params.live.ToString());

            HelperFunctions.DevLog("username = " + username);
            StreamCallBacks.onReceiveStreamLink?.Invoke(username);
        } else if (ContainParam(uri, Params.prerecorded.ToString())) {

            HelperFunctions.DevLog("GetPrerecordedParameters");

            string slug = GetParam(uri, Params.prerecorded.ToString());

            HelperFunctions.DevLog("slug = " + slug);
            StreamCallBacks.onReceivePrerecordedLink?.Invoke(slug);
        }
    }

    private bool ContainParam(Uri uri, string parameter) {
        return ContainFolder(uri, parameter) || ContainQueryParam(uri, parameter);
    }

    private string GetParam(Uri uri, string parameter) {
        if (ContainQueryParam(uri, parameter)) {
            return GetQueryParam(uri, parameter);
        } else if (ContainFolder(uri, parameter)) {
            return GetFolderId(uri, parameter);
        }
        return null;
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

    private bool ContainQueryParam(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter) != null;
    }

    private string GetQueryParam(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }
}