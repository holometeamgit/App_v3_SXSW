using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

public class ARMessageUI : MonoBehaviour {
    [SerializeField]
    private GameObject RecordSteps;
    [SerializeField]
    private UnityEvent OnHasLastUploadingFile;

    private void OnEnable() {
        if (CallBacks.OnCheckContainLastUploadedARMsg != null && CallBacks.OnCheckContainLastUploadedARMsg.Invoke()) {
            OnHasLastUploadingFile?.Invoke();
        } else {
            RecordSteps.gameObject.SetActive(true);
        }
    }
}
