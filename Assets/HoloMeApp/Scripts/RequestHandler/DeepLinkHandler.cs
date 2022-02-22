using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

/// <summary>
/// Handler for all deeplinks
/// </summary>
public class DeepLinkHandler : MonoBehaviour {
    public enum Params {
        room,
        message,
        live,
        stadium,
        prerecorded,
        username,
        gallery
    }

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    private void DeepLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLog("Deep link: " + uriStr);
        GetContentsParameters(uri);
    }

    private void Awake() {
        DynamicLinksCallBacks.onReceivedDeepLink += DeepLinkActivated;
        Application.deepLinkActivated += DeepLinkActivated;
    }

    private void Start() {

        HelperFunctions.DevLog("Application.absoluteURL: " + Application.absoluteURL);

        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            // Cold start and Application.absoluteURL not null so process Deep Link.
            DeepLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= DeepLinkActivated;
        Application.deepLinkActivated -= DeepLinkActivated;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainParam(uri, Params.room.ToString())) {
            string username = GetParam(uri, Params.room.ToString());
            StreamCallBacks.onReceiveRoomLink?.Invoke(username);
        } else if (ContainParam(uri, Params.username.ToString())) {
            string username = GetParam(uri, Params.username.ToString());
            StreamCallBacks.onReceiveRoomLink?.Invoke(username);
        } else if (ContainParam(uri, Params.message.ToString())) {
            string messageId = GetParam(uri, Params.message.ToString());
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainParam(uri, Params.live.ToString())) {
            string username = GetParam(uri, Params.live.ToString());
            StreamCallBacks.onReceiveStadiumLink?.Invoke(username);
        } else if (ContainParam(uri, Params.stadium.ToString())) {
            string username = GetParam(uri, Params.stadium.ToString());
            StreamCallBacks.onReceiveStadiumLink?.Invoke(username);
        } else if (ContainParam(uri, Params.prerecorded.ToString())) {
            string slug = GetParam(uri, Params.prerecorded.ToString());
            StreamCallBacks.onReceivePrerecordedLink?.Invoke(slug);
        } else if (ContainParam(uri, Params.gallery.ToString())) {
            GalleryNotificationConstructor.OnShow?.Invoke();
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