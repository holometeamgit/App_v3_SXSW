using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;
using WindowManager.Extenject;

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
        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Exit", ImmediateInterruption);
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Return", null);
        GeneralPopUpData data = new GeneralPopUpData("Before you go...", "If you exit before processing has completed, you will lose your AR message", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
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
