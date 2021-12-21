using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening room
/// </summary>
public class RoomBtn : MonoBehaviour {

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        RoomTutorialConstructor.OnActivated?.Invoke(false);
        StreamOverlayConstructor.onActivatedAsRoomBroadcaster?.Invoke(true);

        AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyGoLive, new Dictionary<string, string>() { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID } });

    }
}
