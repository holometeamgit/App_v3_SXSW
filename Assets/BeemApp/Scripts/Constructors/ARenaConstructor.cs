using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: class in progress
/// </summary>
public class ARenaConstructor : MonoBehaviour {
    [SerializeField]
    private PnlViewingExperience _pnlViewingExperience;

    public static Action onRunTutorial = delegate { };
    public static Action onShowTapToPlaceMessage = delegate { };
    public static Action onShowPinchToZoomMessage = delegate { };
    public static Action onPlaced = delegate { };
    public static Action onHideScanMessage = delegate { };
    public static Action<StreamJsonData.Data, bool> onActivateForPreRecorded = delegate { };
    public static Action<ARMsgJSON.Data> onActivateForARMessaging = delegate { };
    public static Action<string, string, bool> onActivateForStreaming = delegate { };
    public static Action onDeactivate = delegate { };

    private void OnEnable() {
        onRunTutorial += OnRunTutorial;
        onShowTapToPlaceMessage += OnShowTapToPlaceMessage;
        onShowPinchToZoomMessage += OnShowPinchToZoomMessage;
        onPlaced += OnPlaced;
        onHideScanMessage += OnHideScanMessage;
        onActivateForPreRecorded += OnActivateForPreRecorded;
        onActivateForARMessaging += OnActivateForARMessaging;
        onActivateForStreaming += OnActivateForStreaming;
        onDeactivate += OnDeactivate;
    }

    private void OnDisable() {
        onRunTutorial -= OnRunTutorial;
        onShowTapToPlaceMessage -= OnShowTapToPlaceMessage;
        onShowPinchToZoomMessage -= OnShowPinchToZoomMessage;
        onPlaced -= OnPlaced;
        onHideScanMessage -= OnHideScanMessage;
        onActivateForPreRecorded -= OnActivateForPreRecorded;
        onActivateForARMessaging -= OnActivateForARMessaging;
        onActivateForStreaming -= OnActivateForStreaming;
        onDeactivate -= OnDeactivate;
    }

    private void OnRunTutorial() {
        _pnlViewingExperience.RunTutorial();
    }

    private void OnShowTapToPlaceMessage() {
        _pnlViewingExperience.ShowTapToPlaceMessage();
    }

    private void OnShowPinchToZoomMessage() {
        _pnlViewingExperience.ShowPinchToZoomMessage();
    }

    private void OnPlaced() {
        _pnlViewingExperience.OnPlaced();
    }

    private void OnHideScanMessage() {
        _pnlViewingExperience.HideScanMessage();
    }

    private void OnActivateForPreRecorded(StreamJsonData.Data data, bool isTeaser) {
        _pnlViewingExperience.ActivateForPreRecorded(data, isTeaser);
    }

    private void OnActivateForARMessaging(ARMsgJSON.Data data) {
        _pnlViewingExperience.ActivateForARMessaging(data);
    }

    private void OnActivateForStreaming(string channelName, string streamID, bool isRoom) {
        _pnlViewingExperience.ActivateForStreaming(channelName, streamID, isRoom);
    }

    private void OnDeactivate() {
        _pnlViewingExperience.StopExperience();
    }
}
