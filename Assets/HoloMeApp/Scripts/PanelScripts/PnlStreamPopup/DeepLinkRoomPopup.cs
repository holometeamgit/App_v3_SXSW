using UnityEngine;
using TMPro;

/// <summary>
/// UI popup for opening rooms
/// </summary>
public class DeepLinkRoomPopup : MonoBehaviour {

    [SerializeField]
    private Color _highlightMSGColor;
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _subtitleText;
    [SerializeField]
    private TMP_Text _usersCountText;
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

    private const long USER_NOT_FOUND_CODE = 404;
    private string _user;

    /// <summary>
    /// Call share event for current room
    /// </summary>
    public void Share() {
        StreamCallBacks.onShareRoomLink?.Invoke(_user);
    }

    /// <summary>
    /// Call onOpenRoom event for open current room
    /// </summary>
    public void EnterRoom() {
        StreamCallBacks.onOpenRoom?.Invoke();
    }

    /// <summary>
    /// Show DeepLinkRoomData
    /// </summary>
    /// <param name="deepLinkRoomData"></param>
    public void Show(DeepLinkRoomData deepLinkRoomData) {
        gameObject.SetActive(true);

        _user = deepLinkRoomData.User;

        _titleText.text = deepLinkRoomData.Title;
        _subtitleText.text = deepLinkRoomData.Description;

        _titleText.gameObject.SetActive(deepLinkRoomData.Title.Length > 0);
        _subtitleText.gameObject.SetActive(deepLinkRoomData.Description.Length > 0);
        _usersCountText.gameObject.SetActive(deepLinkRoomData.UserCountTxt);

        _btnClose.SetActive(deepLinkRoomData.CloseBtn);
        _btnShare.SetActive(deepLinkRoomData.User.Length > 0);
        _btnEnterRoom.SetActive(deepLinkRoomData.EnterBtn);

        _streamerCountUpdater.StartCheck(deepLinkRoomData.User, true);

        _streamerCountUpdater.OnCountUpdated += UpdateUserCount;

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
