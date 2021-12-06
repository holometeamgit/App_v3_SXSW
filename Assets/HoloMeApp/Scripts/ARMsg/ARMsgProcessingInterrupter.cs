using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgProcessingInterrupter : MonoBehaviour {
    [SerializeField] Switcher _interruptSwitcher;

    public void Interrupt() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
            "If you exit before processing has completed, you will lose your AR message",
            "Exit", "Return",
            () => {
                ImmediateInterruption();
            }, null, false);
    }

    public void ImmediateInterruption() {
        CallBacks.OnDeleteLastARMsgActions?.Invoke();
        CallBacks.OnCancelAllARMsgActions?.Invoke();
    }

    private void OnEnable() {
        CallBacks.OnCancelAllARMsgActions += OnInterrupt;
    }

    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }

    private void OnDisable() {
        CallBacks.OnCancelAllARMsgActions -= OnInterrupt;
    }
}
