using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Beem.Permissions;

public class PnlRoomBroadcastHoldingScreen : MonoBehaviour
{
 /*   [SerializeField] TMP_Text title;
    [SerializeField] GameObject RectInfo;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] VideoUploader videoUploader;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptable;
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    [SerializeField] ContentLinkHandler contentLinkHandler;
    [SerializeField] PermissionController _permissionController;

    private string currentRoomId = "";

    private const int TIME_DELAY = 5000;

    bool liveRoomWasFound;

    //calling from SwitcherToRoomHolding on scene. TODO need move in controller
    public void CheckRoom() {
        HelperFunctions.DevLog("CheckRoom");
        if (!gameObject.activeInHierarchy)
            return;
        liveRoomWasFound = false;
        if (contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            currentRoomId = contentLinkHandler.PopContentId();
            HelperFunctions.DevLog("currentRoomId " + currentRoomId);
        }
        if (string.IsNullOrWhiteSpace(currentRoomId)) {
            RectInfo.SetActive(true);
            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Delay(TIME_DELAY).ContinueWith((_) => CheckRoom(), taskScheduler);
        } else {
            RequestRoom();
        }
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
    }

    private void RequestRoom() {
        if (string.IsNullOrWhiteSpace(currentRoomId))
            return;
        webRequestHandler.Get(GetRequestRoomUrl(currentRoomId),
            (code, body) => GetRoomCallBack(body),
            (code, body) => RepeatGetRoom(),
            true);
    }

    private void GetRoomCallBack(string roomBody) {
        if (!gameObject.activeInHierarchy)
            return;

        HelperFunctions.DevLog(roomBody);
        RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(roomBody);
        if (roomJsonData.status != StreamJsonData.Data.LIVE_ROOM_STR) {
            RepeatGetRoom(roomJsonData.user);
        } else {
            if (!_permissionController.CheckCameraMicAccess()) {
                RepeatGetRoom(roomJsonData.user);
                return;
            }

            liveRoomWasFound = true;
            uiThumbnailsController.PlayLiveStream(roomJsonData.agora_channel, roomJsonData.agora_channel, roomJsonData.id, true);
            gameObject.SetActive(false);
        }
    }

    private void RepeatGetRoom() {
        if (!gameObject.activeInHierarchy)
            return;

        RectInfo.SetActive(true);

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(TIME_DELAY).ContinueWith((_) => RequestRoom(), taskScheduler);
    }

    private void RepeatGetRoom(string user) {
        title.text = user;
        RepeatGetRoom();
    }

    private string GetRequestRoomUrl(string id) {
        return serverURLAPIScriptable.ServerURLMediaAPI + videoUploader.GetRoomById.Replace("{id}", id);
    }

    private void OnApplicationFocus(bool focus) {
        if (!gameObject.activeInHierarchy)
            return;
        if (focus)
            CheckRoom();
    }

    private void OnDisable() {
        title.text = "";
        RectInfo.SetActive(false);
        currentRoomId = "";

        if(!liveRoomWasFound)
            ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
    }*/
}
