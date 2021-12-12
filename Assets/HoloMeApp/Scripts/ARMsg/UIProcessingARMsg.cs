using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// UI processing checking class 
/// </summary>
public class UIProcessingARMsg : MonoBehaviour {
    private const float COOLDOWN_CHECK = 5f;
    private Coroutine _processingStarting;

    [SerializeField]
    private UnityEvent OnNextStep;

    private void OnChecked(ARMsgJSON.Data data) {
        if (data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            OnNextStep?.Invoke();
        } else {
            if (gameObject.activeInHierarchy) {
                StartProcessing();
            }
        }
    }

    private void OnEnable() {
        ResumeProcessing();
    }

    private void OnDisable() {
        LeaveProcessing();
    }

    private void StartProcessing() {
        if (_processingStarting != null)
            StopCoroutine(_processingStarting);
        _processingStarting = StartCoroutine(CheckingProcessing());
    }

    private IEnumerator CheckingProcessing() {
        yield return new WaitForSeconds(COOLDOWN_CHECK);
        CallBacks.OnARMsgProcessingCheck?.Invoke();
    }

    private void ResumeProcessing() {
        CallBacks.OnARMsgByIdReceived += OnChecked;
        StartProcessing();
    }

    private void LeaveProcessing() {
        CallBacks.OnARMsgByIdReceived -= OnChecked;
        CallBacks.OnCancelLastGetARMsgById?.Invoke();
        if (_processingStarting != null)
            StopCoroutine(_processingStarting);
        _processingStarting = null;
    }

#if UNITY_ANDROID
    void OnApplicationFocus(bool focus) {
        if (focus) {
            ResumeProcessing();
        } else {
            LeaveProcessing();
        }
    }
#endif

#if UNITY_EDITOR || UNITY_IOS
    void OnApplicationPause(bool pause) {
        if (pause) {
            LeaveProcessing();
        } else {
            ResumeProcessing();
        }
    }
#endif

}
