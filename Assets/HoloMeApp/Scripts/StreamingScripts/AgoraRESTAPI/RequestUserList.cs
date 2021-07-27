using System;
using System.Collections.Generic;

public class RequestUserList : RestRequest
{
    public GetUserListRequestResponse GetUserListResponseData { get; private set; }

    public string ChannelName { set { requestString = $"/dev/v1/channel/user/{AgoraController.AppId}/{value}"; } }

    public override void OnSuccess(string result)
    {
        GetUserListResponseData = OnResponseReturned<GetUserListRequestResponse>(result);
        base.OnSuccess(result);
    }
}

[Serializable]
public class GetUserListRequestData
{
    public bool channelExist;
    public int mode;
    public List<string> users = new List<string>();
}

[Serializable]
public class GetUserListRequestResponse
{
    public bool success;
    public GetUserListRequestData data = new GetUserListRequestData();
}


