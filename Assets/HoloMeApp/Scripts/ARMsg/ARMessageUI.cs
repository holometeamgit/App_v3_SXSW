using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

public class ARMessageUI : MonoBehaviour
{
    [SerializeField] GameObject RecordSteps;
    [SerializeField] UnityEvent OnHasLastUploadingFile;

    private void OnEnable() {
        if(CallBacks.OnCheckContainLastUploadedARMsg != null && CallBacks.OnCheckContainLastUploadedARMsg.Invoke()) {
            OnHasLastUploadingFile?.Invoke();
        } else {
            RecordSteps.gameObject.SetActive(true);
        }
    }
}
