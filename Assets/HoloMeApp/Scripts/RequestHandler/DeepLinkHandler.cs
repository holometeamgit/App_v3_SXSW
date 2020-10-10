using UnityEngine;
using System;

public class DeepLinkHandler : MonoBehaviour {
    public static DeepLinkHandler Instance { get; private set; }

    public Action<string> VerificationDeepLinkActivated;
    public Action<string, string> PasswordResetConfirmDeepLinkActivated;

    private const string signUpVerication = "verification";
    private const string passWordResetConfirm = "passwordresetconfirm";

    public void OnDeepLinkActivated(string uriStr) {

        Uri uri = new Uri(uriStr);
        //example
        //beemholomedl://verification?code=string
        //beemholomedl://passwordresetconfirm?uid=string&token=string

        Debug.Log(uriStr);

        switch (uri.Host) {
            case signUpVerication:
                GetVerificationParameters(uri);
                break;
            case passWordResetConfirm:
                GetPasswordResetConfirmParameters(uri);
                break;
        }
    }

    private void Start() {
        if (Instance == null) {
            Instance = this;
            Application.deepLinkActivated += OnDeepLinkActivated;
            CheckStartDeepLink();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
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

}
