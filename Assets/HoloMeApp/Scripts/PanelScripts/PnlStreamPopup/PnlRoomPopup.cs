using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PnlRoomPopup : MonoBehaviour {

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

    [SerializeField]
    SwipePopUp _swipePopUp;
    [SerializeField]
    CanvasGroup _swipePopUpCanvasGroup;

    public void Share() {
        StreamCallBacks.onShareRoom?.Invoke();
    }

    public void EnterRoom() {
        StreamCallBacks.onOpenRoom?.Invoke();
    }

    private void Awake() {
        StreamCallBacks.onShowPopUpRoomOnline += ShowCurrentlyOnline;
        StreamCallBacks.onUpdateUserCount += UpdateUserCount;
        StreamCallBacks.onShowPopUpRoomOffline += ShowCurrentlyOffline;
        StreamCallBacks.onShowPopUpRoomEnded += ShowNoLongerOnline;

        _swipePopUp.onHid += OnClose;
        _swipePopUp.onShowed += StartInteraction;
        _swipePopUp.onStartHiding += StopInteraction;
        _swipePopUp.onStartShowing += StopInteraction;
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

    private void UpdateUserCount(int personInside) {
        if (personInside < 1)
            _usersCountText.text = "No person inside";
        else {
            _usersCountText.text = DataStringConverter.GetItems(personInside, "person", "people") + " inside";
        }
    }

    private void StartInteraction() {
        _swipePopUpCanvasGroup.interactable = true;
    }

    private void StopInteraction() {
        _swipePopUpCanvasGroup.interactable = false;
    }

    private void OnClose() {
        StreamCallBacks.onPopUpClosed?.Invoke();
    }

    private void OnDestroy() {
        StreamCallBacks.onShowPopUpRoomOnline -= ShowCurrentlyOnline;
        StreamCallBacks.onShowPopUpRoomOffline -= ShowCurrentlyOffline;
        StreamCallBacks.onUpdateUserCount -= UpdateUserCount;
        StreamCallBacks.onShowPopUpRoomEnded -= ShowNoLongerOnline;

        _swipePopUp.onHid -= OnClose;
        _swipePopUp.onShowed -= StartInteraction;
        _swipePopUp.onStartHiding -= StopInteraction;
        _swipePopUp.onStartShowing -= StopInteraction;
    }
}
