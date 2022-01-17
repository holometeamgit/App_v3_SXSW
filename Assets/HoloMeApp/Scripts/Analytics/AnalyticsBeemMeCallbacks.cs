using Beem.ARMsg;
using UnityEngine;

public class AnalyticsBeemMeCallbacks : MonoBehaviour {

    void Start() {
        CallBacks.OnStartRecord += () => AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeRecordStarted);
        CallBacks.OnARMsgUpdloaded += () => AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeUploadComplete);
        CallBacks.OnARMsgByIdReceived += x => { if (x.processing_status == ARMsgJSON.Data.COMPETED_STATUS) { AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeConversionComplete); } };
    }
}
