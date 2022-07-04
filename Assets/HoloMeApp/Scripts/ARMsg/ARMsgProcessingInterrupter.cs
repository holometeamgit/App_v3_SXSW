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
        WarningConstructor.ActivateDoubleButton("Are you sure you\nwant to quit?",
            "You will lose your hologram",
             "quit", "stay",
            () => {
                ImmediateInterruption();
            }, null, false);
    }

    public void ImmediateInterruption() {
        CallBacks.OnDeleteLastARMsgActions?.Invoke();
        CallBacks.OnCancelAllARMsgActions?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
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
