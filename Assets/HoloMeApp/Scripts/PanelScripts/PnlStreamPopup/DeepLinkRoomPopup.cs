using UnityEngine;
using TMPro;

/// <summary>
/// UI popup for opening rooms
/// </summary>
public class DeepLinkRoomPopup : MonoBehaviour {
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
    private GameObject _btnClose;
    [SerializeField]
    private GameObject _btnShare;
    [SerializeField]
    private GameObject _btnEnterRoom;
    [SerializeField]
    private StreamerCountUpdater _streamerCountUpdater;
    [SerializeField]
    private SwipePopUp _swipePopUp;

    private RoomJsonData _data;

    /// <summary>
    /// Call share event for current room
    /// </summary>
    public void Share() {
        StreamCallBacks.onShareRoomLink?.Invoke(_data.user);
    }

    /// <summary>
    /// Call onOpenRoom event for open current room
    /// </summary>
    public void EnterRoom() {
        StreamCallBacks.onPlayRoom?.Invoke(_data);
        DeepLinkRoomConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Show DeepLinkRoomData
    /// </summary>
    /// <param name="deepLinkRoomData"></param>
    public void Show(DeepLinkRoomData deepLinkRoomData) {
        gameObject.SetActive(true);

        _swipePopUp.onHid -= Close;

        _data = deepLinkRoomData.Data;

        _titleText.text = deepLinkRoomData.Title;
        _subtitleText.text = deepLinkRoomData.Description;

        _title.SetActive(deepLinkRoomData.Title.Length > 0);
        _subtitle.SetActive(deepLinkRoomData.Description.Length > 0);
        _usersCount.SetActive(deepLinkRoomData.Online);

        _btnClose.SetActive(deepLinkRoomData.CloseBtn);
        _btnShare.SetActive(deepLinkRoomData.ShareBtn);
        _btnEnterRoom.SetActive(deepLinkRoomData.Online);

        if (deepLinkRoomData.Data != null && deepLinkRoomData.Online) {
            _streamerCountUpdater.StartCheck(deepLinkRoomData.Data.user, true);
            _streamerCountUpdater.OnCountUpdated += UpdateUserCount;
        }

        _swipePopUp.Show();

        _swipePopUp.onHid += Close;
    }

    /// <summary>
    /// Hide Popup
    /// </summary>
    public void Hide() {
        _swipePopUp.Hide();
        _streamerCountUpdater.StopCheck();
        _streamerCountUpdater.OnCountUpdated -= UpdateUserCount;
    }

    private void Close() {
        _swipePopUp.onHid -= Close;
        gameObject.SetActive(false);
    }

    private void UpdateUserCount(int personInside) {
        if (personInside < 1)
            _usersCountText.text = "No person inside";
        else {
            _usersCountText.text = DataStringConverter.GetItems(personInside, "person", "people") + " inside";
        }
    }
}
