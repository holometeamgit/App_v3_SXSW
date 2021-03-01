using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class DeepLinkHandler : MonoBehaviour {
    public static DeepLinkHandler Instance { get; private set; }

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;
    public Action<ServerAccessToken> OnCompleteSSOLoginGetted;

    public const string ROOM = "room";
    public const string ROOM_ID_PARAMETTR_NAME = "roomid";

    private const string signUpVerication = "verification";
    private const string passWordResetConfirm = "passwordresetconfirm";
    private const string completeSSOLogin = "complete-login";

    public void OnDeepLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);
        //example
        //beemholomedl://verification?code=string
        //beemholomedl://passwordresetconfirm?uid=string&token=string
        //beemholomedl://room?roomid=string

        HelperFunctions.DevLog("Deep link: " + uriStr);

        switch (uri.Host) {
            case signUpVerication:
                GetVerificationParameters(uri);
                break;
            case passWordResetConfirm:
                GetPasswordResetConfirmParameters(uri);
                break;
            case completeSSOLogin:
                GetCompleteSSOLoginParameters(uri);
                break;
        }
    }

    public void OnDynamicLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);

        HelperFunctions.DevLog("Dynamic link: " + uriStr);
        GetRoomParameters(uri);
    }

    private void Start() {
        if (Instance == null) {
            Instance = this;
            Application.deepLinkActivated += OnDeepLinkActivated;
            DynamicLinksCallBacks.onReceivedDeepLink += OnDynamicLinkActivated;
            CheckStartDeepLink();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        Application.deepLinkActivated -= OnDeepLinkActivated;
        DynamicLinksCallBacks.onReceivedDeepLink -= OnDynamicLinkActivated;
    }

    private void CheckStartDeepLink() {
        if (!string.IsNullOrEmpty(Application.absoluteURL)) {
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    private void GetVerificationParameters(Uri uri) {
        Debug.Log("GetVerificationParameters");

        string code = HttpUtility.ParseQueryString(uri.Query).Get("code");

        Debug.Log(code);
        //if (!string.IsNullOrWhiteSpace(code))
        VerificationDeepLinkActivated?.Invoke(code);
    }

    private void GetPasswordResetConfirmParameters(Uri uri) {
        Debug.Log("GetPasswordResetConfirmParameters " + uri.Host);
        string uid = HttpUtility.ParseQueryString(uri.Query).Get("uid");
        string token = HttpUtility.ParseQueryString(uri.Query).Get("token");

        Debug.Log("uid " + uid + " token " + token);
        //if (!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(token))
        PasswordResetConfirmDeepLinkActivated?.Invoke(uid, token);
    }

    private void GetCompleteSSOLoginParameters(Uri uri) {
        Debug.Log("GetCompleteSSOLoginParameters " + uri.Host);
        string refresh = HttpUtility.ParseQueryString(uri.Query).Get("refresh");
        string access = HttpUtility.ParseQueryString(uri.Query).Get("access");

        if (string.IsNullOrWhiteSpace(refresh) || string.IsNullOrWhiteSpace(access))
            return;

        ServerAccessToken serverAccessToken = new ServerAccessToken(refresh, access);


        //Debug.Log("uid " + uid + " token " + token);
        //if (!string.IsNullOrWhiteSpace(uid) && !string.IsNullOrWhiteSpace(token))
        OnCompleteSSOLoginGetted?.Invoke(serverAccessToken);
    }

    private void GetRoomParameters(Uri uri) {
        HelperFunctions.DevLog("GetRoomParameters");

        string roomId = uri.Host;

        HelperFunctions.DevLog("roomId = " + roomId);
        StreamCallBacks.onRoomLinkReceived?.Invoke(roomId);
    }
}
