using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deep Link Room Constructor
/// </summary>
public class DeepLinkRoomConstructor : MonoBehaviour {

    [SerializeField]
    private DeepLinkRoomPopup _deepLinkRoomPopup;
    [SerializeField]
    private DeepLinkChecker _popupShowChecker;
    [SerializeField]
    private AgoraController _agoraController;
    [SerializeField]
    private Color _highlightMSGColor;

    private RoomJsonData _data;

    public static Action<RoomJsonData> OnShow;
    public static Action OnShowAlert;
    public static Action<long> OnShowError;
    public static Action OnHide;

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
        OnShowAlert += ShowNoLongerData;
        OnShowError += ShowError;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
        OnShowAlert -= ShowNoLongerData;
        OnShowError -= ShowError;
    }

    private void Show(RoomJsonData data) {
        OnReceivedARMessageData(data, ShowOnlineData);
    }

    private void OnReceivedARMessageData(RoomJsonData data, Action<RoomJsonData> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ShowOnlineData(RoomJsonData data) {
        string onlineText = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is online";
        string offlineText = $"<color=#{ ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is currently offline";

        string title = data.status == StreamJsonData.Data.LIVE_ROOM_STR ? onlineText : offlineText;
        string description = string.Empty;
        string user = data.status == StreamJsonData.Data.LIVE_ROOM_STR ? data.user : string.Empty;
        bool userCountText = data.status == StreamJsonData.Data.LIVE_ROOM_STR;
        bool closeBtn = false;
        bool enterBtn = data.status == StreamJsonData.Data.LIVE_ROOM_STR;

        DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, user, userCountText, closeBtn, enterBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
        _data = data;
    }

    private void ShowNoLongerData() {
        if (_data != null) {

            string noNolngerOfflineText = _data != null ? $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{_data.user}</color> is no longer live" : string.Empty;

            string title = noNolngerOfflineText;
            string description = string.Empty;
            string user = string.Empty;
            bool userCountText = false;
            bool closeBtn = true;
            bool enterBtn = false;

            DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, user, userCountText, closeBtn, enterBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
        }
    }

    private void ShowError(long error) {
        if (error == 404) {

            string title = "THIS USER DOES NOT EXIST";
            string description = "Please make sure that the link you received is correct.";
            string user = string.Empty;
            bool userCountText = false;
            bool closeBtn = true;
            bool enterBtn = false;

            DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, user, userCountText, closeBtn, enterBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
        }
    }

    private void Hide() {
        _deepLinkRoomPopup.Hide();
    }
}
