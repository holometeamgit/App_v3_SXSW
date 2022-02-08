using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

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

    private void Awake() {
        Construct();
    }

    private void Construct() {
        _pnlRoomPopupController = new PnlRoomPopupController(_roomPopupShowChecker, _streamerCountUpdater);
    }
}
