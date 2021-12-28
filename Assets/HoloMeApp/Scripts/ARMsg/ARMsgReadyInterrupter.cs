using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// ARMsgReadyInterrupter. Invoke GenericError for closing ready screen
/// </summary>
public class ARMsgReadyInterrupter : MonoBehaviour {
    [SerializeField]
    private Switcher _interruptSwitcher;

    [SerializeField] UnityEvent OnShare;

    private void OnEnable() {
        CallBacks.OnAllARMsgСanceled += OnInterrupt;
    }

    private void OnDisable() {
        CallBacks.OnAllARMsgСanceled -= OnInterrupt;
    }

    /// <summary>
    /// Request GenericError for interrupting
    /// </summary>
    public void Interrupt() {
        WarningConstructor.ActivateDoubleButton("Before you go...",
            "If you exit without sharing it, your Beem will be lost",
            "Copy link and exit", "Share",
            () => {
                GUIUtility.systemCopyBuffer = CallBacks.OnGetLastARMsgShareLink?.Invoke();
                CallBacks.OnCancelAllARMsgActions?.Invoke();
            }, () => {
                OnShare?.Invoke();
            }
            , false);
    }
    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }
}