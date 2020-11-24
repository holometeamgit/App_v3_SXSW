using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignUpStartVerificationHandler : MonoBehaviour
{
    [SerializeField] PnlEmailVerification pnlEmailVerification;
    [SerializeField] DeepLinkHandler deepLinkHandler;

    [SerializeField] UnityEvent OnStartVerification;

    private void Awake() {

    }

    public void VerificationDeepLinkActivated(string key) {
        OnStartVerification.Invoke();
        pnlEmailVerification.Verify(key);
    }

    private void OnEnable() {
        deepLinkHandler.VerificationDeepLinkActivated += VerificationDeepLinkActivated;
    }

    private void OnDisable() {
        deepLinkHandler.VerificationDeepLinkActivated -= VerificationDeepLinkActivated;
    }
}
