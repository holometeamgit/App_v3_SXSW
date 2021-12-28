using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgUploadingRepeater : MonoBehaviour {
    [SerializeField] ARMsgProcessingInterrupter _processingInterrupter;

    private void OnUploadingError() {
        WarningConstructor.ActivateDoubleButton("Connection Interupted",
            "We lost connection whilst uploading your Beem",
            "Exit", "Retry",
            () => {
                _processingInterrupter.ImmediateInterruption();
            }, () => {
                CallBacks.OnUpdloadingUIOpened?.Invoke();
            }
            , false);
    }

    private void OnEnable() {
        CallBacks.OnARMsgUploadedError += OnUploadingError;
    }

    private void OnDisable() {
        CallBacks.OnARMsgUploadedError -= OnUploadingError;
    }
}
