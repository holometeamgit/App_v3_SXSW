using Beem.ARMsg;
using UnityEngine;

/// <summary>
/// This class assigns relevant callbacks for analytics and Beem Me feature
/// </summary>
public class AnalyticsBeemMeCallbacks : MonoBehaviour {

    private void Start() {
        CallBacks.OnStartRecord += OnStartRecordHandler;
        CallBacks.OnARMsgUpdloaded += OnARMsgUpdloadedHandler;
        CallBacks.OnARMsgByIdReceived += OnARMsgByIdReceivedHandler;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= OnStartRecordHandler;
        CallBacks.OnARMsgUpdloaded -= OnARMsgUpdloadedHandler;
        CallBacks.OnARMsgByIdReceived -= OnARMsgByIdReceivedHandler;
    }

    private void OnStartRecordHandler() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeRecordStarted);
    }

    private void OnARMsgUpdloadedHandler() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeUploadComplete);
    }

    private void OnARMsgByIdReceivedHandler(ARMsgJSON.Data data) {
        if (data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyBeemMeConversionComplete);
        };
    }
}
