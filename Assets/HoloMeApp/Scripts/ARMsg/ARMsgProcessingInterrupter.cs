using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

/// <summary>
/// ARMsgProcessingInterrupter. Invoke GenericError for closing processing
/// </summary>
public class ARMsgProcessingInterrupter : MonoBehaviour {
    [SerializeField]
    private Switcher _interruptSwitcher;

    /// <summary>
    /// Request GenericError for interrupting
    /// </summary>
    public void Interrupt() {
        WarningConstructor.ActivateDoubleButton("Before you go...",
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
