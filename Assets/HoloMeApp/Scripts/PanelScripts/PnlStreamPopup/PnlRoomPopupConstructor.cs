using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlRoomPopupConstructor : MonoBehaviour
{
    [SerializeField] PnlRoomPopup _pnlRoomPopup;
    [SerializeField] RoomPopupShowChecker _roomPopupShowChecker;
    private PnlRoomPopupController _pnlRoomPopupController;

    private void Awake() {

    }

    private void Construct() {
        _pnlRoomPopupController = new PnlRoomPopupController(_roomPopupShowChecker);
        _pnlRoomPopupController.Test();
    }
}
