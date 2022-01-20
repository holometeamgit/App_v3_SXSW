using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// ARMessageUI. Select current step when ARmsg openning
/// </summary>
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

    /// <summary>
    /// Close ARMessage Steps
    /// </summary>
    public void CloseARMessageSteps() {
        MenuConstructor.OnActivated?.Invoke(true);
        //HomeScreenConstructor.OnActivated?.Invoke(true);
        CallBacks.OnActivated?.Invoke(false);
    }
}
