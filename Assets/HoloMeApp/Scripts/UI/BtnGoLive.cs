using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.Permissions;

public class BtnGoLive : MonoBehaviour {
    [SerializeField] UnityEvent OnGoLive;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    public void GoLive() {
        if (!permissionController.CheckCameraMicAccess()) {
            return;
        }

        AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });
        OnGoLive.Invoke();

    }
}
