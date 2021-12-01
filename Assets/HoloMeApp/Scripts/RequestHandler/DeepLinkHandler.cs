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
        if (ContainParameter(uri, DynamicLinkParameters.Folder.username.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameterId(uri, DynamicLinkParameters.Folder.username.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onReceiveRoomLink?.Invoke(userName);
        } else if (ContainParameter(uri, DynamicLinkParameters.Folder.message.ToString())) {

            HelperFunctions.DevLog("GetMessageParameters");

            string messageId = GetParameterId(uri, DynamicLinkParameters.Folder.message.ToString());

            HelperFunctions.DevLog("messageId = " + messageId);
            StreamCallBacks.onReceiveARMsgLink?.Invoke(messageId);
        } else if (ContainParameter(uri, DynamicLinkParameters.Folder.live.ToString())) {

            HelperFunctions.DevLog("GetLiveParameters");

            string username = GetParameterId(uri, DynamicLinkParameters.Folder.live.ToString());

            HelperFunctions.DevLog("username = " + username);
            StreamCallBacks.onReceiveStreamLink?.Invoke(username);
        } else if (ContainParameter(uri, DynamicLinkParameters.Folder.prerecorded.ToString())) {

            HelperFunctions.DevLog("GetPrerecordedParameters");

            string slug = GetParameterId(uri, DynamicLinkParameters.Folder.prerecorded.ToString());

            HelperFunctions.DevLog("slug = " + slug);
            StreamCallBacks.onReceivePrerecordedLink?.Invoke(slug);
        }
    }

    private bool ContainParameter(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter) != null;
    }

    private string GetParameterId(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }
}