using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening tutorial for room
/// </summary>
public class RoomTutorialBtn : MonoBehaviour {

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        SettingsConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
        RoomTutorialConstructor.OnActivated?.Invoke(true);

        AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });

    }
}
