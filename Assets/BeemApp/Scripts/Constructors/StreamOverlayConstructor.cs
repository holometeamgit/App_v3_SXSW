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

    public static Action onActivatedAsStadiumBroadcaster = delegate { };
    public static Action onActivatedAsRoomBroadcaster = delegate { };
    public static Action<string, string, bool> onActivatedAsViewer = delegate { };
    public static Action onDeactivatedAsBroadcaster = delegate { };

    public static Action onDeactivate = delegate { };

    private void OnEnable() {
        onActivatedAsStadiumBroadcaster += OnActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster += OnActivatedAsRoomBroadcaster;
        onActivatedAsViewer += OnActivatedAsViewer;
        onDeactivate += OnDeactivate;
        onDeactivatedAsBroadcaster += OnDeactivateAsBroadcaster;
    }

    private void OnDisable() {
        onActivatedAsStadiumBroadcaster -= OnActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster -= OnActivatedAsRoomBroadcaster;
        onActivatedAsViewer -= OnActivatedAsViewer;
        onDeactivate -= OnDeactivate;
        onDeactivatedAsBroadcaster -= OnDeactivateAsBroadcaster;
    }

    private void OnActivatedAsStadiumBroadcaster() {
        _pnlStreamOverlay.OpenAsStreamer();
    }

    private void OnActivatedAsRoomBroadcaster() {
        _pnlStreamOverlay.OpenAsRoomBroadcaster();
    }

    private void OnActivatedAsViewer(string channelName, string streamID, bool isRoom) {
        _pnlStreamOverlay.OpenAsViewer(channelName, streamID, isRoom);
    }

    private void OnDeactivate() {
        _pnlStreamOverlay.Deactivate();
    }

    private void OnDeactivateAsBroadcaster() {
        _pnlStreamOverlay.CloseAsStreamer();
    }

}
