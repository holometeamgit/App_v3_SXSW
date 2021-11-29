using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgProcessingInterrupter : MonoBehaviour {
    [SerializeField] Switcher _interruptSwitcher;

    private void OnEnable() {
        CallBacks.OnCancelAllARMsgActions += OnInterrupt;
    }

    public void Interrupt() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
            "If you exit before processing has completed, you will lose your AR message",
            "Exit", "Return",
            () => {
                CallBacks.OnDeleteLastARMsgActions?.Invoke();
                CallBacks.OnCancelAllARMsgActions?.Invoke();
            }, null, false);
    }
    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }
}
