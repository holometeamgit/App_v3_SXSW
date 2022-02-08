using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening tutorial for room
/// </summary>
public class RoomTutorialBtn : MonoBehaviour {

    private PermissionController _permissionController = new PermissionController();

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        _permissionController.CheckCameraMicAccess(() => {
            SettingsConstructor.OnActivated?.Invoke(false);
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            StreamCallBacks.onCloseComments?.Invoke();
            RoomTutorialConstructor.OnActivated?.Invoke(true);
            AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });
        });
    }
}
