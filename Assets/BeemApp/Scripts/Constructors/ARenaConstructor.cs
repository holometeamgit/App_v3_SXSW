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
    public static Action<StreamJsonData.Data, bool> OnShowPrerecorded = delegate { };
    public static Action<ARMsgJSON.Data> OnShowARMessaging = delegate { };
    public static Action<StreamJsonData.Data> OnShowStadium = delegate { };
    public static Action<RoomJsonData> OnShowRoom = delegate { };

    public static Action OnHide = delegate { };

    private void OnEnable() {
        onRunTutorial += OnRunTutorial;
        onShowTapToPlaceMessage += OnShowTapToPlaceMessage;
        onShowPinchToZoomMessage += OnShowPinchToZoomMessage;
        onPlaced += OnPlaced;
        onHideScanMessage += OnHideScanMessage;
        OnShowPrerecorded += ShowPrerecorded;
        OnShowARMessaging += ShowARMessaging;
        OnShowStadium += ShowStadium;
        OnShowRoom += ShowRoom;
        OnHide += Hide;
    }

    private void OnDisable() {
        onRunTutorial -= OnRunTutorial;
        onShowTapToPlaceMessage -= OnShowTapToPlaceMessage;
        onShowPinchToZoomMessage -= OnShowPinchToZoomMessage;
        onPlaced -= OnPlaced;
        onHideScanMessage -= OnHideScanMessage;
        OnShowPrerecorded -= ShowPrerecorded;
        OnShowARMessaging -= ShowARMessaging;
        OnShowStadium -= ShowStadium;
        OnShowRoom -= ShowRoom;
        OnHide -= Hide;
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

    private void ShowPrerecorded(StreamJsonData.Data data, bool isTeaser) {
        _pnlViewingExperience.ShowPrerecorded(data, isTeaser);
    }

    private void ShowARMessaging(ARMsgJSON.Data data) {
        _pnlViewingExperience.ShowARMessaging(data);
    }

    private void ShowStadium(StreamJsonData.Data data) {
        _pnlViewingExperience.ShowStadium(data);
    }

    private void ShowRoom(RoomJsonData data) {
        _pnlViewingExperience.ShowRoom(data);
    }

    private void Hide() {
        _pnlViewingExperience.Hide();
    }
}
