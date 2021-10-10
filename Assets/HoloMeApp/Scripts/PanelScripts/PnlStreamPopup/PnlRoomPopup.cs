using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PnlRoomPopup : MonoBehaviour {

    public Action onClose;
    public Action onEnterRoom;

    [SerializeField]
    Color _highlightMSGColor;
    [SerializeField]
    TMP_Text _titleText;
    [SerializeField]
    TMP_Text _subtitleText;
    [SerializeField]
    TMP_Text _usersCountText;

    [SerializeField]
    GameObject _title;
    [SerializeField]
    GameObject _subtitle;
    [SerializeField]
    GameObject _usersCount;

    [SerializeField]
    GameObject _btnClose, _btnShare, _btnEnterRoom;

    private RoomJsonData _lastroomJsonData;

    public void Close() {
        onClose?.Invoke();
    }

    public void Share() {
        StreamCallBacks.onGetLastRoomLink?.Invoke();
    }

    public void EnterRoom() {
        onEnterRoom?.Invoke();
    }

    private void ShowNoLongerOnline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color> is no longer live", _highlightMSGColor, username);

        _subtitle.SetActive(false);
        _usersCount.SetActive(false);

        _btnClose.SetActive(true);
        _btnShare.SetActive(false);
        _btnEnterRoom.SetActive(false);
    }

    private void ShowCurrentlyOffline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color>’s room is currently offline", _highlightMSGColor, username);

        _subtitle.SetActive(true);
        _usersCount.SetActive(false);

        _btnClose.SetActive(false);
        _btnShare.SetActive(true);
        _btnEnterRoom.SetActive(false);
    }

    private void ShowCurrentlyOnline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color>’s room is online", _highlightMSGColor, username);

        _subtitle.SetActive(false);
        _usersCount.SetActive(true);

        _btnClose.SetActive(false);
        _btnShare.SetActive(false);
        _btnEnterRoom.SetActive(true);
    }

    private void UpdateUserCount(long personInside) {
        if (personInside < 1)
            _usersCountText.text = "No person inside";
        else {
            _usersCountText.text = DataStringConverter.GetItems(personInside, "person", "people") + " inside";
        }
    }
}
