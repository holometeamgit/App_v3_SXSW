using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Zenject;

/// <summary>
/// Constructor for PnlRoomPopup and PnlRoomPopupController
/// need for initialization
/// </summary>
public class PnlRoomPopupConstructor : MonoBehaviour {
    [SerializeField]
    private PnlRoomPopup _pnlRoomPopup;
    [SerializeField]
    private DeepLinkChecker _roomPopupShowChecker;
    [SerializeField]
    private StreamerCountUpdater _streamerCountUpdater;
    private PnlRoomPopupController _pnlRoomPopupController;

    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    private void Awake() {
        Construct();
    }

    private void Construct() {
        _pnlRoomPopupController = new PnlRoomPopupController(_roomPopupShowChecker, _streamerCountUpdater, _userWebManager);

        StreamCallBacks.onShowPopUpRoomOnline += _pnlRoomPopup.ShowCurrentlyOnline;
        StreamCallBacks.onShowPopUpRoomOffline += _pnlRoomPopup.ShowCurrentlyOffline;
        StreamCallBacks.onShowPopUpRoomEnded += _pnlRoomPopup.ShowNoLongerOnline;
        StreamCallBacks.onUserDoesntExist += _pnlRoomPopup.ShowUserDoesntExist;
        StreamCallBacks.onClosePopUp += _pnlRoomPopup.Hide;

    }

    private void OnDestroy() {
        StreamCallBacks.onShowPopUpRoomOnline -= _pnlRoomPopup.ShowCurrentlyOnline;
        StreamCallBacks.onShowPopUpRoomOffline -= _pnlRoomPopup.ShowCurrentlyOffline;
        StreamCallBacks.onShowPopUpRoomEnded -= _pnlRoomPopup.ShowNoLongerOnline;
        StreamCallBacks.onUserDoesntExist -= _pnlRoomPopup.ShowUserDoesntExist;
        StreamCallBacks.onClosePopUp -= _pnlRoomPopup.Hide;
    }
}
