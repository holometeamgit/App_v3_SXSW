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

    public void StartCheck(string channelName)
    {
        if (requestUserList == null)
        {
            requestUserList = new RequestUserList();
            requestUserList.OnSuccessAction += UpdateCountText;
        }

        requestUserList.ChannelName = channelName;
        StartCoroutine(UpdateCountRoutine());
        HelperFunctions.DevLog("Getting user count routine started");
    }

    IEnumerator UpdateCountRoutine()
    {
        while (true)
        {
            UpdateCount();
            yield return new WaitForSeconds(5);
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
        HelperFunctions.DevLog("Got user count back");
        var responseData = requestUserList.GetUserListResponseData;
        txtCount.text = responseData.data.users.Count.ToString();
        waitingForResponse = false;
    }

}
