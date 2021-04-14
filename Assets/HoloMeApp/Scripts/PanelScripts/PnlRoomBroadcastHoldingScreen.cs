using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class PnlRoomBroadcastHoldingScreen : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] GameObject RectInfo;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] VideoUploader videoUploader;
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptable;
    [SerializeField] AccountManager accountManager;
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    [SerializeField] ContentLinkHandler contentLinkHandler;

    private string currentRoomId = "";

    private const int TIME_DELAY = 5000;

    bool liveRoomWasFound;

    private void RequestRoom() {
        webRequestHandler.GetRequest(GetRequestRoomUrl(currentRoomId),
            (code, body) => GetRoomCallBack(body),
            (code, body) => RepeatGetRoom(),
            accountManager.GetAccessToken().access);
    }

    private void GetRoomCallBack(string roomBody) {
        if (!gameObject.activeInHierarchy)
            return;

        HelperFunctions.DevLog(roomBody);
        RoomJsonData roomJsonData = JsonUtility.FromJson<RoomJsonData>(roomBody);
        if(roomJsonData.status != StreamJsonData.Data.LIVE_ROOM_STR) {
            RepeatGetRoom(roomJsonData.agora_channel);
        } else {
            liveRoomWasFound = true;
            uiThumbnailsController.PlayLiveStream(roomJsonData.agora_channel, roomJsonData.agora_channel, roomJsonData.id);
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

    private void OnEnable() {
        liveRoomWasFound = false;
        if (contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            currentRoomId = contentLinkHandler.PopContentId();
        }
        if (string.IsNullOrWhiteSpace(currentRoomId)) {
            RectInfo.SetActive(true);
        } else {
            RequestRoom();
        }
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
    }

    private void OnDisable() {
        title.text = "";
        RectInfo.SetActive(false);
        currentRoomId = "";

        if(!liveRoomWasFound)
            ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
    }
}
