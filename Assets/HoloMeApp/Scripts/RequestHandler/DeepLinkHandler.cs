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
        GetContentsParameters(uri);
    }

    private void Awake() {
        DynamicLinksCallBacks.onReceivedDeepLink += OnDynamicLinkActivated;
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
    }


    private void GetContentsParameters(Uri uri) {
        if (ContainParameter(uri, serverURLAPIScriptableObject.StreamId)) {
            HelperFunctions.DevLog("GetStreamParameters");

            string streamId = GetParameter(uri, serverURLAPIScriptableObject.StreamId);

            HelperFunctions.DevLog("streamId = " + streamId);
            StreamCallBacks.onStreamLinkReceived?.Invoke(streamId);
        } else if (ContainParameter(uri, serverURLAPIScriptableObject.Username)) {

            HelperFunctions.DevLog("GetRoomParameters");

            string userName = GetParameter(uri, serverURLAPIScriptableObject.Username);

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