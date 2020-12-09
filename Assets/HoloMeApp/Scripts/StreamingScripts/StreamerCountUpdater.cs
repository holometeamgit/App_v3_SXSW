using System.Collections;
using TMPro;
using UnityEngine;

public class StreamerCountUpdater : MonoBehaviour
{
    [SerializeField]
    AgoraRequests agoraRequests;

    [SerializeField]
    TextMeshProUGUI txtCount;

    RequestUserList requestUserList;

    bool waitingForResponse;

    Coroutine updateRoutine;

    public void StartCheck(string channelName)
    {
        if (requestUserList == null)
        {
            requestUserList = new RequestUserList();
            requestUserList.OnSuccessAction += UpdateCountText;
            requestUserList.OnFailedAction += () => waitingForResponse = false;
        }

        requestUserList.ChannelName = channelName;
        StopCheck();
        updateRoutine = StartCoroutine(UpdateCountRoutine());
        HelperFunctions.DevLog("Getting user count routine started");
    }

    public void StopCheck()
    {
        if (updateRoutine != null)
            StopCoroutine(updateRoutine);

        txtCount.text = "0";
    }

    IEnumerator UpdateCountRoutine()
    {
        while (true)
        {
            UpdateCount();
            yield return new WaitForSeconds(30);
        }
    }

    void UpdateCount()
    {
        if (!waitingForResponse)
        {
            HelperFunctions.DevLog("Sending user count request");
            waitingForResponse = true;
            agoraRequests.MakeGetRequest(requestUserList);
        }
    }

    void UpdateCountText()
    {
        var responseData = requestUserList.GetUserListResponseData;
        //if (responseData == null)
        //    Debug.LogError("response was null");
        //if (responseData.data == null)
        //    Debug.LogError("data was null");
        txtCount.text = responseData?.data?.audience?.Count.ToString() ?? "0";
        waitingForResponse = false;
        HelperFunctions.DevLog("Got user count back");
    }

}
