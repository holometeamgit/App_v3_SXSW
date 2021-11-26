using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamOverlayConstructor : MonoBehaviour {
    [SerializeField]
    private PnlStreamOverlay _pnlStreamOverlay;

    public static Action<bool> onActivatedAsLiveBroadcaster = delegate { };
    public static Action<bool> onActivatedAsRoomBroadcaster = delegate { };
    public static Action<bool> onActivatedAsLiveViewer = delegate { };
    public static Action<bool> onActivatedAsRoomViewer = delegate { };

    private void OnEnable() {
        onActivatedAsLiveBroadcaster += ActivatedAsLiveBroadcaster;
        onActivatedAsRoomBroadcaster += ActivatedAsRoomBroadcaster;
        onActivatedAsLiveViewer += ActivatedAsLiveViewer;
        onActivatedAsRoomViewer += ActivatedAsRoomViewer;
    }

    private void OnDisable() {
        onActivatedAsLiveBroadcaster -= ActivatedAsLiveBroadcaster;
        onActivatedAsRoomBroadcaster -= ActivatedAsRoomBroadcaster;
        onActivatedAsLiveViewer -= ActivatedAsLiveViewer;
        onActivatedAsRoomViewer -= ActivatedAsRoomViewer;
    }

    private void ActivatedAsLiveBroadcaster(bool status) {
        if (status) {
            _pnlStreamOverlay.OpenAsStreamer();
        } else {
            _pnlStreamOverlay.CloseAsStreamer();
        }
    }

    private void ActivatedAsRoomBroadcaster(bool status) {
        if (status) {
            _pnlStreamOverlay.OpenAsRoomBroadcaster();
        } else {
            _pnlStreamOverlay.CloseAsStreamer();
        }
    }

    private void ActivatedAsLiveViewer(bool status) {
        if (status) {

        }
    }

    private void ActivatedAsRoomViewer(bool status) {

    }

}
