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

        HelperFunctions.DevLogError("Dynamic link: " + uriStr);
        GetContentsParameters(uri);
    }

    public void OnDeepLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLogError("Deep link: " + uriStr);
        GetContentsParameters(uri);
    }

    private void OnEnable() {

        HelperFunctions.DevLogError("Awake");

        HelperFunctions.DevLogError("Application.absoluteURL: " + Application.absoluteURL);

        DynamicLinksCallBacks.onReceivedDeepLink += OnDynamicLinkActivated;
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            // Cold start and Application.absoluteURL not null so process Deep Link.
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDisable() {
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainParam(uri, Params.room.ToString())) {
            string userName = GetParam(uri, Params.room.ToString());
            StreamCallBacks.onReceiveRoomLink?.Invoke(userName);
        } else if (ContainParam(uri, Params.message.ToString())) {
            string messageId = GetParam(uri, Params.message.ToString());
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainParam(uri, Params.live.ToString())) {
            string username = GetParam(uri, Params.live.ToString());
            StreamCallBacks.onReceiveStreamLink?.Invoke(username);
        } else if (ContainParam(uri, Params.prerecorded.ToString())) {
            string slug = GetParam(uri, Params.prerecorded.ToString());
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