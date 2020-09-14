using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpVerificationTester : MonoBehaviour {

    [SerializeField] DeepLinkHandler DeepLinkHandler;
    [SerializeField] string code;

    [ContextMenu("SendVerificationCode")]
    public void SendVerificationCode() {
        Debug.Log("SendVerificationCode " + code);
        DeepLinkHandler.OnDeepLinkActivated("beemholomedl://verification?code=" + code);
    }
}
