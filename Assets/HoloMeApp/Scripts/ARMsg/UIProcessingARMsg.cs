using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;
using Beem.ARMsg;

public class UIProcessingARMsg : MonoBehaviour
{
    private const float COOLDOWN_CHECK = 5f;

    [SerializeField] private UnityEvent OnNextStep; 

    private void OnChecked(ARMsgJSON.Data data) {
        if(data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            OnNextStep?.Invoke();
        }
    }

    private void OnEnable() {
        CallBacks.OnARMsgByIdReceived += OnChecked;
        StartCoroutine(CheckingProcessing());
    }

    private void OnDisable() {
        CallBacks.OnARMsgByIdReceived -= OnChecked;
        StopAllCoroutines();
    }

    private IEnumerator CheckingProcessing() {

        while (true) {
            yield return new WaitForSeconds(COOLDOWN_CHECK);
            CallBacks.OnARMsgProcessingCheck?.Invoke();
        }
    }
}
