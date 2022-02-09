using UnityEngine;
using TMPro;

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

    private const string USER_NOT_FOUND_CODE = "404";

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
        StreamCallBacks.onUpdateUserCount += UpdateUserCount;

        _swipePopUp.onHid += OnPopUpClosed;
        _swipePopUp.onShowed += StartInteraction;
        _swipePopUp.onStartHiding += StopInteraction;
        _swipePopUp.onStartShowing += StartInteraction;
        _swipePopUp.onStartShowing += OnPopUpStartOpen;
    }

    /// <summary>
    /// Show Window
    /// </summary>
    /// <param name="data"></param>
    public void Show(DeepLinkRoomData data) {
        switch (data.GetSettings()) {
            case DeepLinkRoomData.Settings.NotExist:
                ShowUserDoesntExist(data.GetText());
                break;
            case DeepLinkRoomData.Settings.Ended:
                ShowNoLongerOnline(data.GetText());
                break;
            case DeepLinkRoomData.Settings.Online:
                ShowCurrentlyOnline(data.GetText());
                break;
            default:
                ShowCurrentlyOffline(data.GetText());
                break;
        }
    }

    /// <summary>
    /// User doesn'tExist
    /// </summary>
    /// <param name="errorCode"></param>
    private void ShowUserDoesntExist(string errorCode) {
        if (errorCode != USER_NOT_FOUND_CODE) {
            return;
        }

        _titleText.text = "THIS USER DOES NOT EXIST";
        _subtitleText.text = "Please make sure that the link you received is correct.";

        _subtitle.SetActive(true);
        _usersCount.SetActive(false);

        _btnClose.SetActive(true);
        _btnShare.SetActive(false);
        _btnEnterRoom.SetActive(false);

        _swipePopUp.Show();
    }

    /// <summary>
    /// User No Longer online
    /// </summary>
    /// <param name="username"></param>
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

    /// <summary>
    /// User is Offline
    /// </summary>
    /// <param name="username"></param>
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

    /// <summary>
    /// User is online
    /// </summary>
    /// <param name="username"></param>
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

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
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
        OnPopUpStartClosing();
    }

    private void OnPopUpStartOpen() {
        StreamCallBacks.onPopUpStartOpen?.Invoke();
    }

    private void OnPopUpStartClosing() {
        StreamCallBacks.onPopUpStartClosing?.Invoke();
    }

    private void OnPopUpClosed() {
        StreamCallBacks.onPopUpClosed?.Invoke();
    }

    private void OnDestroy() {
        StreamCallBacks.onUpdateUserCount -= UpdateUserCount;

        _swipePopUp.onHid -= OnPopUpClosed;
        _swipePopUp.onShowed -= StartInteraction;
        _swipePopUp.onStartHiding -= StopInteraction;
        _swipePopUp.onStartShowing -= StartInteraction;
        _swipePopUp.onStartShowing -= OnPopUpStartOpen;
    }
}
