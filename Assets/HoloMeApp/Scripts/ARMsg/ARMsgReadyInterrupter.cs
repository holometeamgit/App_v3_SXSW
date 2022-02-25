using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;
using Beem.UI;

/// <summary>
/// ARMsgReadyInterrupter. Invoke GenericError for closing ready screen
/// </summary>
public class ARMsgReadyInterrupter : MonoBehaviour, IARMsgDataView {
    [SerializeField]
    private Switcher _interruptSwitcher;

    [SerializeField] UnityEvent OnShare;

    private ARMsgJSON.Data currentData;

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
                GUIUtility.systemCopyBuffer = currentData.share_link;
                MenuConstructor.OnActivated?.Invoke(true);
                ARMsgRecordConstructor.OnActivated?.Invoke(false);
                ARMsgRecordConstructor.OnActivated?.Invoke(true);
                CallBacks.OnCancelAllARMsgActions?.Invoke();

            }, () => {
                OnShare?.Invoke();
            }
            , false);
    }
    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
    }
}