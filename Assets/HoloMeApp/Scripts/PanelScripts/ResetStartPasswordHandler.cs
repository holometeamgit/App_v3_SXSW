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

    }

    public void AddVerificationData(string uid, string token) {
        OnStartResetPassword.Invoke();
        pnlResetPassword.AddVerificationData(uid, token);
    }

    private void OnEnable() {
        deepLinkHandler.PasswordResetConfirmDeepLinkActivated += AddVerificationData;
    }

    private void OnDisable() {
        deepLinkHandler.PasswordResetConfirmDeepLinkActivated -= AddVerificationData;
    }
}
