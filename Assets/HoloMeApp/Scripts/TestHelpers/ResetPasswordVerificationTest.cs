using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPasswordVerificationTest : MonoBehaviour
{
    [SerializeField] DeepLinkHandler DeepLinkHandler;
    [SerializeField] string uid;
    [SerializeField] string token;

    [ContextMenu("SendVerificationCode")]
    public void SendVerificationCode() {
        Debug.Log("SendVerificationCode " + uid + " " + token);
        DeepLinkHandler.OnDeepLinkActivated("beemholomedl://passwordresetconfirm?uid=" + uid + "&token=" + token);
    }
}
