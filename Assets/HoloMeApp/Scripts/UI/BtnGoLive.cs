using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class BtnGoLive : MonoBehaviour
{
    [SerializeField] UnityEvent OnGoLive;
    [SerializeField] PermissionController permissionController;

    public void GoLive() {
        if (permissionController.CheckCameraMicAccess())
        {
            AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance },AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });
            OnGoLive.Invoke();
        }
    }
}
