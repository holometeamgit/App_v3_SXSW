using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Zenject;
using System;

/// <summary>
/// Constructor for PnlRoomPopup and PnlRoomPopupController
/// need for initialization
/// </summary>
public class DeepLinkRoomConstructor : MonoBehaviour {
    [SerializeField]
    private PnlRoomPopup _pnlRoomPopup;
    [SerializeField]
    private DeepLinkChecker _roomPopupShowChecker;
    [SerializeField]
    private StreamerCountUpdater _streamerCountUpdater;

    private DeepLinkRoomPopupController _pnlRoomPopupController;

    private UserWebManager _userWebManager;

    public static Action<DeepLinkRoomData> OnShow = delegate { };
    public static Action OnHide = delegate { };

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    private void Awake() {
        Construct();
    }

    private void Construct() {
        _pnlRoomPopupController = new DeepLinkRoomPopupController(_roomPopupShowChecker, _streamerCountUpdater, _userWebManager);

        OnShow += _pnlRoomPopup.Show;
        OnHide += _pnlRoomPopup.Hide;

    }

    private void OnDestroy() {
        OnShow -= _pnlRoomPopup.Show;
        OnHide -= _pnlRoomPopup.Hide;
    }
}
