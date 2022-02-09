using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;
using Beem.UI;
using WindowManager.Extenject;

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

        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Copy link and exit", CopyAndExit);
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Share", () => OnShare?.Invoke());
        GeneralPopUpData data = new GeneralPopUpData("Before you go...", "If you exit without sharing it, your Beem will be lost", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
    }

    private void CopyAndExit() {
        GUIUtility.systemCopyBuffer = currentData.share_link;
        HomeConstructor.OnActivated?.Invoke(true);
        BottomMenuConstructor.OnActivated?.Invoke(true);
        ARMsgRecordConstructor.OnActivated?.Invoke(false);
        CallBacks.OnCancelAllARMsgActions?.Invoke();
    }

    private void OnInterrupt() {
        _interruptSwitcher.Switch();
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
    }
}