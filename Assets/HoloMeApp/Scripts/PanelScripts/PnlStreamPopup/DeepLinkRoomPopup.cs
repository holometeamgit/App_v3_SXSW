using UnityEngine;
using TMPro;
using Beem.Permissions;
using Zenject;

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

    private UserWebManager _userWebManager;

    private PermissionController _permissionController = new PermissionController();

    private RoomJsonData _data;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

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
        if (_data.user == _userWebManager.GetUsername()) {
            WarningConstructor.ActivateSingleButton("Viewing as stream host",
                "Please connect to the stream using a different account");

            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkRoomConstructor.OnHide?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(_data.agora_channel, _data.id, true);
            PnlRecord.CurrentUser = _data.user;
        });

    }

    /// <summary>
    /// Close popup
    /// </summary>
    public void Close() {
        DeepLinkRoomConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Show DeepLinkRoomData
    /// </summary>
    /// <param name="deepLinkRoomData"></param>
    public void Show(DeepLinkRoomData deepLinkRoomData) {
        gameObject.SetActive(true);

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
    }

    /// <summary>
    /// Hide Popup
    /// </summary>
    public void Hide() {
        _swipePopUp.onHid += OnClose;
        _swipePopUp.Hide();
        _streamerCountUpdater.StopCheck();
        _streamerCountUpdater.OnCountUpdated -= UpdateUserCount;
    }

    private void OnClose() {
        _swipePopUp.onHid -= OnClose;
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
