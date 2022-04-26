using UnityEngine;
using TMPro;
using Beem.Permissions;
using Zenject;
using System.Collections.Generic;

/// <summary>
/// UI popup for opening deeplinks
/// </summary>
public class DeepLinkPopup : MonoBehaviour {
    [SerializeField]
    private Color _highlightMSGColor;
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _subtitleText;
    [SerializeField]
    private TMP_Text _usersCountText;
    [SerializeField]
    private TMP_Text _enterText;

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
    private ShareLinkController _shareController = new ShareLinkController();

    private IData _data;

    private const string LINK_TITLE = "You have been invited to {0}'s {1}";
    private const string LINK_DESCRIPTION = "Click the link below to join {0}'s {1}";
    private const string ROOM = "Room";
    private const string STADIUM = "Stadium";

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Call share event for current room
    /// </summary>
    public void Share() {
        if (!string.IsNullOrWhiteSpace(_data.GetShareLink)) {
            string title = string.Format(LINK_TITLE, _data.GetUsername, _data is RoomJsonData ? ROOM : STADIUM);
            string description = string.Format(LINK_DESCRIPTION, _data.GetUsername, _data is RoomJsonData ? ROOM : STADIUM);
            string msg = title + "\n" + description + "\n" + _data.GetShareLink;
            _shareController.ShareLink(msg);
        }
    }

    /// <summary>
    /// Call onOpenRoom event for open current room
    /// </summary>
    public void EnterRoom() {
        if (_data.GetUsername == _userWebManager.GetUsername()) {
            WarningConstructor.ActivateSingleButton("Viewing as stream host",
                "Please connect to the stream using a different account");

            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(_data.GetUsername, _data.GetId, _data is RoomJsonData);
            PnlRecord.CurrentUser = _data.GetUsername;
        });

    }

    /// <summary>
    /// Close popup
    /// </summary>
    public void Close() {
        DeepLinkStreamConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Show DeepLinkRoomData
    /// </summary>
    /// <param name="deepLinkUIData"></param>
    public void Show(DeepLinkUIData deepLinkUIData) {
        gameObject.SetActive(true);

        _data = deepLinkUIData.Data;

        _titleText.text = string.Format(deepLinkUIData.Title, ColorUtility.ToHtmlStringRGBA(_highlightMSGColor), _data.GetUsername);
        _subtitleText.text = deepLinkUIData.Description;
        _enterText.text = deepLinkUIData.ButtonText;


        _title.SetActive(deepLinkUIData.Title.Length > 0);
        _subtitle.SetActive(deepLinkUIData.Description.Length > 0);
        _usersCount.SetActive(deepLinkUIData.Online);

        _btnClose.SetActive(deepLinkUIData.CloseBtn);
        _btnShare.SetActive(deepLinkUIData.ShareBtn);
        _btnEnterRoom.SetActive(deepLinkUIData.Online);

        if (_data != null && deepLinkUIData.Online) {
            _streamerCountUpdater.StartCheck(_data.GetUsername, true);
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
