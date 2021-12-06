using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

public class ARMsgReadyInterrupter : MonoBehaviour {
    [SerializeField] Switcher _interruptSwitcher;

    [SerializeField] UnityEvent OnShare;

    private void OnEnable() {
        CallBacks.OnAllARMsgСanceled += OnInterrupt;
    }

    public void Interrupt() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
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