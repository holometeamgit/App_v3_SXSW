using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgReadyInterrupter : MonoBehaviour {
    [SerializeField] Switcher _interruptSwitcher;

    private void OnEnable() {
        CallBacks.OnAllARMsgСanceled += OnInterrupt;
    }

    public void Interrupt() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
            "If you exit you could lose your AR message if you don’t share the link.",
            "Copy link and exit", "Return",
            () => {
                GUIUtility.systemCopyBuffer = CallBacks.OnGetLastARMsgShareLink?.Invoke();
                CallBacks.OnCancelAllARMsgActions?.Invoke();
            }, null, false);
    }
    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }
}