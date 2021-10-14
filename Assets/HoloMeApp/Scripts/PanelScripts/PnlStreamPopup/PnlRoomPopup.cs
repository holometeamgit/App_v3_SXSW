using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI popup for opening rooms
/// </summary>
public class PnlRoomPopup : MonoBehaviour {

    [SerializeField]
    private Color _highlightMSGColor;
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _subtitleText;
    [SerializeField]
    private TMP_Text _usersCountText;

    [SerializeField]
    private GameObject _title;
    [SerializeField]
    private GameObject _subtitle;
    [SerializeField]
    private GameObject _usersCount;

    [SerializeField]
    private GameObject _btnClose, _btnShare, _btnEnterRoom;

    [SerializeField]
    private SwipePopUp _swipePopUp;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// Call share event for current room
    /// </summary>
    public void Share() {
        StreamCallBacks.onShareRoom?.Invoke();
    }

    /// <summary>
    /// Call onOpenRoom event for open current room
    /// </summary>
    public void EnterRoom() {
        StreamCallBacks.onOpenRoom?.Invoke();
    }

    private void Awake() {
        StreamCallBacks.onShowPopUpRoomOnline += ShowCurrentlyOnline;
        StreamCallBacks.onUpdateUserCount += UpdateUserCount;
        StreamCallBacks.onShowPopUpRoomOffline += ShowCurrentlyOffline;
        StreamCallBacks.onShowPopUpRoomEnded += ShowNoLongerOnline;
        StreamCallBacks.onClosePopUp += Hide;

        _swipePopUp.onHid += OnPopUpClose;
        _swipePopUp.onShowed += StartInteraction;
        _swipePopUp.onStartHiding += StopInteraction;
        _swipePopUp.onStartShowing += StartInteraction;
        _swipePopUp.onStartShowing += OnPopUpStartOpen;
    }

    private void ShowNoLongerOnline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color> is no longer live",
            ColorUtility.ToHtmlStringRGBA(_highlightMSGColor), username);

        _subtitle.SetActive(false);
        _usersCount.SetActive(false);

        _btnClose.SetActive(true);
        _btnShare.SetActive(false);
        _btnEnterRoom.SetActive(false);

        _swipePopUp.Show();
    }

    private void ShowCurrentlyOffline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color>’s room is currently offline",
            ColorUtility.ToHtmlStringRGBA(_highlightMSGColor), username);

        _subtitle.SetActive(true);
        _usersCount.SetActive(false);

        _btnClose.SetActive(false);
        _btnShare.SetActive(true);
        _btnEnterRoom.SetActive(false);

        _swipePopUp.Show();
    }

    private void ShowCurrentlyOnline(string username) {
        _titleText.text = string.Format("<color=#{0}>{1}</color>’s room is online",
            ColorUtility.ToHtmlStringRGBA(_highlightMSGColor), username);

        _subtitle.SetActive(false);
        _usersCount.SetActive(true);

        _btnClose.SetActive(false);
        _btnShare.SetActive(false);
        _btnEnterRoom.SetActive(true);

        _swipePopUp.Show();
    }

    private void Hide() {
        _swipePopUp.Hide();
    }

    private void UpdateUserCount(int personInside) {
        if (personInside < 1)
            _usersCountText.text = "No person inside";
        else {
            _usersCountText.text = DataStringConverter.GetItems(personInside, "person", "people") + " inside";
        }
    }

    private void StartInteraction() {
        _canvasGroup.blocksRaycasts = true;
    }

    private void StopInteraction() {
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnPopUpStartOpen() {
        StreamCallBacks.onPopUpStartOpen?.Invoke();
    }

    private void OnPopUpClose() {
        StreamCallBacks.onPopUpClosed?.Invoke();
    }

    private void OnDestroy() {
        StreamCallBacks.onShowPopUpRoomOnline -= ShowCurrentlyOnline;
        StreamCallBacks.onShowPopUpRoomOffline -= ShowCurrentlyOffline;
        StreamCallBacks.onUpdateUserCount -= UpdateUserCount;
        StreamCallBacks.onShowPopUpRoomEnded -= ShowNoLongerOnline;
        StreamCallBacks.onClosePopUp -= Hide;

        _swipePopUp.onHid -= OnPopUpClose;
        _swipePopUp.onShowed -= StartInteraction;
        _swipePopUp.onStartHiding -= StopInteraction;
        _swipePopUp.onStartShowing -= StartInteraction;
        _swipePopUp.onStartShowing -= OnPopUpStartOpen;
    }
}
