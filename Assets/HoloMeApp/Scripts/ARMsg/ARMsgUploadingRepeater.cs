using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;
using WindowManager.Extenject;

public class ARMsgUploadingRepeater : MonoBehaviour {
    [SerializeField] ARMsgProcessingInterrupter _processingInterrupter;

    private void OnUploadingError() {
        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Exit", _processingInterrupter.ImmediateInterruption);
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Retry", () => CallBacks.OnUpdloadingUIOpened?.Invoke());
        GeneralPopUpData data = new GeneralPopUpData("Connection Interupted", "We lost connection whilst uploading your Beem", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
    }

    private void OnEnable() {
        CallBacks.OnARMsgUploadedError += OnUploadingError;
    }

    private void OnDisable() {
        CallBacks.OnARMsgUploadedError -= OnUploadingError;
    }
}
