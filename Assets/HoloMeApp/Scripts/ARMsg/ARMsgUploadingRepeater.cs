using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class ARMsgUploadingRepeater : MonoBehaviour {
    [SerializeField]
    private ARMsgProcessingInterrupter _processingInterrupter;

    [SerializeField]
    private GameObject _errorInfo;

    [SerializeField]
    private GameObject _defaultInfo;

    /// <summary>
    /// RetryUploading
    /// </summary>
    public void RetryUploading() {
        CallBacks.OnUpdloadingUIOpened?.Invoke();
        ShowError(false);
    }

    private void OnUploadingError() {
        ShowError(true);
    }

    private void OnEnable() {
        CallBacks.OnARMsgUploadedError += OnUploadingError;
    }

    private void OnDisable() {
        CallBacks.OnARMsgUploadedError -= OnUploadingError;
        ShowError(false);
    }

    private void ShowError(bool isShow) {
        _errorInfo.SetActive(isShow);
        _defaultInfo.SetActive(!isShow);
    }
}
