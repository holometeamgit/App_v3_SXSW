using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignUpStartVerificationHandler : MonoBehaviour
{
    [SerializeField] PnlEmailVerification pnlEmailVerification;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    [SerializeField] UnityEvent OnStartResetPassword;

    private void Awake() {
        deepLinkHandler.VerificationDeepLinkActivated += VerificationDeepLinkActivated;
    }

    public void VerificationDeepLinkActivated(string key) {
        pnlEmailVerification.gameObject.SetActive(true);
        deepLinkHandler.VerificationDeepLinkActivated -= VerificationDeepLinkActivated;
        pnlEmailVerification.Verify(key);
    }
}
