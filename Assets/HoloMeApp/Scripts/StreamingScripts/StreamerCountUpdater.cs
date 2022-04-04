using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StreamerCountUpdater : MonoBehaviour {
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

    private AgoraRequests _agoraRequests;

    [Inject]
    public void Construct(AgoraRequests agoraRequests) {
        _agoraRequests = agoraRequests;
    }

    public void StartCheck(string channelName, bool isRoom) {
        if (!gameObject.activeInHierarchy)
            return;

        this.isRoom = isRoom;

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
            _agoraRequests.MakeGetRequest(requestUserList);
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
            if (isRoom) {
                userCount = responseData.data.users.Count - 1;
                foreach (string user in responseData.data.users) {
                    if (user == RequestCloudRecordAcquire.CLOUD_RECORD_UID) //Subtract cloud record server as viewer
                    {
                        userCount -= 1;
                    }
                }
            } else {
                userCount = responseData.data.audience_total; //Subtract streamer's own value if room
            }

            if (userCount < 0) { //Set to 0 is negative value
                userCount = 0;
            }
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
