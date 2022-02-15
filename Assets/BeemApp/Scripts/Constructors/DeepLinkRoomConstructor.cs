using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private Color _highlightMSGColor;

    private RoomJsonData _data;

    public static Action<RoomJsonData> OnShow;
    public static Action<long> OnShowError;
    public static Action OnHide;

    private bool isDisabled;
    private const int DELAY = 5000;

    private void OnEnable() {
        isDisabled = false;
        OnShow += Show;
        OnHide += Hide;
        OnShowError += ShowError;
    }

    private void OnDisable() {
        isDisabled = true;
        OnShow -= Show;
        OnHide -= Hide;
        OnShowError -= ShowError;
    }

    private void Show(RoomJsonData data = null) {
        if (!isDisabled) {
            if (data != null) {
                OnReceivedARMessageData(data, ShowData);
            } else {
                OnReceivedARMessageData(data, (roomJsondata) => ShowNoLongerOnline());
            }
        }
    }

    private void OnReceivedARMessageData(RoomJsonData data, Action<RoomJsonData> onSuccessTask) {
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private async void ShowData(RoomJsonData data) {
        if (data.status == StreamJsonData.Data.LIVE_ROOM_STR) {
            ShowOnline(data);
        } else {
            ShowOffline(data);
        }

        if (!isDisabled) {
            await Task.Delay(DELAY);
            StreamCallBacks.onReceiveRoomLink?.Invoke(data.user);
        }

    }

    private void ShowNoLongerOnline() {
        if (_data != null) {
            string title = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{_data.user}</color> is no longer live";
            string description = string.Empty;
            bool online = false;
            bool closeBtn = true;
            bool shareBtn = false;
            DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, _data, online, closeBtn, shareBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
        }
    }

    private void ShowOnline(RoomJsonData data) {
        string title = $"<color=#{ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is online";
        string description = string.Empty;
        bool online = true;
        bool closeBtn = false;
        bool shareBtn = false;
        _data = data;
        DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, data, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
    }

    private void ShowOffline(RoomJsonData data) {
        string title = $"<color=#{ ColorUtility.ToHtmlStringRGBA(_highlightMSGColor)}>{data.user}</color>’s room is currently offline";
        string description = string.Empty;
        bool online = false;
        bool closeBtn = false;
        bool shareBtn = true;
        DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, data, online, closeBtn, shareBtn);
        _deepLinkRoomPopup.Show(deepLinkRoomData);
    }

    private void ShowError(long error) {
        if (error == 404) {
            string title = "THIS USER DOES NOT EXIST";
            string description = "Please make sure that the link you received is correct.";
            RoomJsonData roomJsonData = null;
            bool userCountText = false;
            bool closeBtn = true;
            bool shareBtn = false;

            DeepLinkRoomData deepLinkRoomData = new DeepLinkRoomData(title, description, roomJsonData, userCountText, closeBtn, shareBtn);
            _deepLinkRoomPopup.Show(deepLinkRoomData);
        }
    }

    private void Hide() {
        isDisabled = true;
        _data = null;
        _deepLinkRoomPopup.Hide();
    }
}
