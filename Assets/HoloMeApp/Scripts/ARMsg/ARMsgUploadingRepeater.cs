using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgUploadingRepeater : MonoBehaviour
{
    [SerializeField] ARMsgProcessingInterrupter _processingInterrupter;

    private void OnUploadingError() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Connection Interupted",
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
        CallBacks.OnARMsgUpdloadedError += OnUploadingError;
    }

    private void OnDisable() {
        CallBacks.OnARMsgUpdloadedError -= OnUploadingError;
    }
}
