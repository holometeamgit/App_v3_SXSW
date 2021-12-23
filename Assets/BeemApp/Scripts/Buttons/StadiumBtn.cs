using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening room
/// </summary>
public class StadiumBtn : MonoBehaviour {

    private PermissionController _permissionController;

    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {

        permissionController.CheckCameraMicAccess(() => {
            SettingsConstructor.OnActivated?.Invoke(false);
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            StreamCallBacks.onCloseComments?.Invoke();
            StreamOverlayConstructor.onActivatedAsStadiumBroadcaster?.Invoke(true);
            ARMessageRoomConstructor.OnActivated?.Invoke(false);
            AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });

        });


    }
}
