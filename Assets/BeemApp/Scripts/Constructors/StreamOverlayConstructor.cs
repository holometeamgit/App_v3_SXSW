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

    public static Action OnHide = delegate { };

    private void OnEnable() {
        onActivatedAsStadiumBroadcaster += OnActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster += OnActivatedAsRoomBroadcaster;
        onActivatedAsViewer += OnActivatedAsViewer;
        OnHide += Hide;
    }

    private void OnDisable() {
        onActivatedAsStadiumBroadcaster -= OnActivatedAsStadiumBroadcaster;
        onActivatedAsRoomBroadcaster -= OnActivatedAsRoomBroadcaster;
        onActivatedAsViewer -= OnActivatedAsViewer;
        OnHide -= Hide;
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

    private void Hide() {
        _pnlStreamOverlay.Deactivate();
    }

}
