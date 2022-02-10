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

    public static Action<BroadcasterData> OnShowAsBroadcaster = delegate { };
    public static Action<StreamJsonData.Data> OnShowAsStadiumViewer = delegate { };
    public static Action<RoomJsonData> OnShowAsRoomViewer = delegate { };

    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void OnEnable() {
        OnShowAsBroadcaster += ShowAsBroadcaster;
        OnShowAsRoomViewer += ShowAsRoomViewer;
        OnShowAsStadiumViewer += ShowAsStadiumViewer;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShowAsBroadcaster -= ShowAsBroadcaster;
        OnShowAsRoomViewer -= ShowAsRoomViewer;
        OnShowAsStadiumViewer -= ShowAsStadiumViewer;
        OnHide -= Hide;
    }

    private void ShowAsBroadcaster(BroadcasterData data) {
        IsActive = true;
        _pnlStreamOverlay.Show(data);
    }

    private void ShowAsRoomViewer(RoomJsonData data) {
        IsActive = true;
        _pnlStreamOverlay.Show(data);
    }

    private void ShowAsStadiumViewer(StreamJsonData.Data data) {
        IsActive = true;
        _pnlStreamOverlay.Show(data);
    }

    private void Hide() {
        IsActive = false;
        _pnlStreamOverlay.Hide();
    }

}
