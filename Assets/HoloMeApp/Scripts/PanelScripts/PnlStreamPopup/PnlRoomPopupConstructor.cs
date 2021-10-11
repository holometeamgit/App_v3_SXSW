using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class PnlRoomPopupConstructor : MonoBehaviour {
    [SerializeField]
    PnlRoomPopup _pnlRoomPopup;
    [SerializeField]
    RoomPopupShowChecker _roomPopupShowChecker;
    [SerializeField]
    StreamerCountUpdater _streamerCountUpdater;
    private PnlRoomPopupController _pnlRoomPopupController;

    private void Awake() {
        Construct();
    }

    private void Construct() {
        _pnlRoomPopupController = new PnlRoomPopupController(_roomPopupShowChecker, _streamerCountUpdater);
        Test();
    }

    private void Test() {
        RoomJsonData roomJsonData = new RoomJsonData();
        roomJsonData.agora_channel = "ivklim21";
        roomJsonData.user = "ivklim21";
        roomJsonData.status = "live_room";
        //StreamCallBacks.onRoomClosed -= OnRoomStreamClosed;

        //TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        //Task.Delay(CHECK_COOLDOWN * 3).ContinueWith(task => {
        //    Debug.Log("Request cancel ");
        //    _cancellationTokenSource.Cancel(true);

        //}, taskScheduler);

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(5000).ContinueWith(task => {
            StreamCallBacks.onRoomDataReceived?.Invoke(roomJsonData);




        }, taskScheduler);

        Task.Delay(10000).ContinueWith(task => {

            StreamCallBacks.onOpenRoom?.Invoke();
            StreamCallBacks.onRoomClosed?.Invoke();

        }, taskScheduler);
    }
}
