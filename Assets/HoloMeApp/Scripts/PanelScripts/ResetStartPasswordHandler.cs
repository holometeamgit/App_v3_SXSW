using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResetStartPasswordHandler : MonoBehaviour {
    [SerializeField] PnlResetPassword pnlResetPassword;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    [SerializeField] UnityEvent OnStartResetPassword;

    private void Awake() {
        deepLinkHandler.PasswordResetConfirmDeepLinkActivated += AddVerificationData;
    }

    public void AddVerificationData(string uid, string token) {
        pnlResetPassword.gameObject.SetActive(true);
        deepLinkHandler.PasswordResetConfirmDeepLinkActivated -= AddVerificationData;
        pnlResetPassword.AddVerificationData(uid, token);
    }
}
