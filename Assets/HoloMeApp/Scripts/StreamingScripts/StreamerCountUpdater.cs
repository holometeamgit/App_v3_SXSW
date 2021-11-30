using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StreamerCountUpdater : MonoBehaviour {
    [SerializeField]
    private AgoraRequests agoraRequests;
    [SerializeField]
    private TextMeshProUGUI txtCount;
    [SerializeField]
    private bool emptyTextIfZero;
    [SerializeField]
    GameObject imageToDisableIfZero;

    private bool isRoom;
    private RequestUserList requestUserList;
    private bool waitingForResponse;
    private Coroutine updateRoutine;

    private const int COUNT_UPDATE_DELAY_SECONDS = 15;

    public Action<int> OnCountUpdated = delegate { };

    public void StartCheck(string channelName, bool isRoom) {
        if (!gameObject.activeInHierarchy)
            return;

        this.isRoom = isRoom;

        agoraRequests = HelperFunctions.GetTypeIfNull<AgoraRequests>(agoraRequests);

        if (requestUserList == null) {
            requestUserList = new RequestUserList();
            requestUserList.OnSuccessAction += UpdateCountText;
            requestUserList.OnFailedAction += () => waitingForResponse = false;
        }

        requestUserList.ChannelName = channelName;
        StopCheck();
        updateRoutine = StartCoroutine(UpdateCountRoutine());
        HelperFunctions.DevLog("Getting user count routine started");
    }

    public void StopCheck() {
        if (updateRoutine != null)
            StopCoroutine(updateRoutine);

        txtCount.text = emptyTextIfZero ? "" : "0";
    }

    IEnumerator UpdateCountRoutine() {
        while (true) {
            UpdateCount();
            yield return new WaitForSeconds(COUNT_UPDATE_DELAY_SECONDS);
        }
    }

    void UpdateCount() {
        if (!waitingForResponse) {
            HelperFunctions.DevLog("Sending user count request");
            waitingForResponse = true;
            agoraRequests.MakeGetRequest(requestUserList);
        }
    }

    void UpdateCountText() {
        var responseData = requestUserList.GetUserListResponseData;

        int userCount = 0;

        if (responseData == null) {
            Debug.LogError("response was null");
        }
        if (responseData.data == null) {
            Debug.LogError("data was null");
        } else {
            userCount = isRoom ? responseData.data.users.Count - 1 : responseData.data.users.Count; //Subtract streamer's own value if room
            foreach (string user in responseData.data.users) {
                if (user == RequestCloudRecordAcquire.CLOUD_RECORD_UID) { //Subtract cloud record server as viewer
                    userCount -= 1;
                }
            }
            if (userCount < 0) //Set to 0 is negative value
                userCount = 0;
        }

        txtCount.text = emptyTextIfZero && userCount == 0 ? "" : userCount.ToString();
        if (imageToDisableIfZero) {
            imageToDisableIfZero.gameObject.SetActive(userCount > 0);
        }
        OnCountUpdated?.Invoke(userCount);
        waitingForResponse = false;
        HelperFunctions.DevLog("Got user count back");
    }
}
