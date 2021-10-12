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

    private void TestDynamicLink(string uriStr) {
        Debug.LogError("OnDynamicLinkActivated: " + uriStr);
        OnDynamicLinkActivated(uriStr);
    }

    private void TestDeepLink(string uriStr) {
        Debug.LogError("OnDeepLinkActivated: " + uriStr);
        OnDynamicLinkActivated(uriStr);
    }

    private void Awake() {
        DynamicLinksCallBacks.onReceivedDeepLink += TestDynamicLink;
        Application.deepLinkActivated += TestDeepLink;
        Debug.LogError("Application.absoluteURL =" + Application.absoluteURL);
        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            // Cold start and Application.absoluteURL not null so process Deep Link.
            OnDynamicLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= TestDynamicLink;
        Application.deepLinkActivated -= TestDeepLink;
    }

    private void GetContentsParameters(Uri uri) {
        if (ContainParameter(uri, DynamicLinkParameters.Parameter.streamId.ToString())) {
            HelperFunctions.DevLog("GetStreamParameters");

            string streamId = GetParameter(uri, DynamicLinkParameters.Parameter.streamId.ToString());

            HelperFunctions.DevLog("streamId = " + streamId);
            StreamCallBacks.onStreamLinkReceived?.Invoke(streamId);
        } else if (ContainParameter(uri, DynamicLinkParameters.Parameter.username.ToString())) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameter(uri, DynamicLinkParameters.Parameter.username.ToString());

            HelperFunctions.DevLog("username = " + userName);
            StreamCallBacks.onUsernameLinkReceived?.Invoke(userName);
        }
    }

    private bool ContainParameter(Uri uri, string parameter) {
        return !string.IsNullOrEmpty(HttpUtility.ParseQueryString(uri.Query).Get(parameter));
    }

    private string GetParameter(Uri uri, string parameter) {
        return HttpUtility.ParseQueryString(uri.Query).Get(parameter);
    }
}