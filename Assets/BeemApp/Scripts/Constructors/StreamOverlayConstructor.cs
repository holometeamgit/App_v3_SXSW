using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: class in progress
/// </summary>
public class StreamOverlayConstructor : MonoBehaviour {
    [SerializeField]
    private PnlStreamOverlay _pnlStreamOverlay;

    public static Action<bool> onActivatedAsStadiumBroadcaster = delegate { };
    public static Action<bool> onActivatedAsRoomBroadcaster = delegate { };

    private void OnEnable() {
        onActivatedAsStadiumBroadcaster += ActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster += ActivatedAsRoomBroadcaster;
    }

    private void OnDisable() {
        onActivatedAsStadiumBroadcaster -= ActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster -= ActivatedAsRoomBroadcaster;
    }

    private void ActivatedAsStadiumBroadcaster(bool status) {
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

}
