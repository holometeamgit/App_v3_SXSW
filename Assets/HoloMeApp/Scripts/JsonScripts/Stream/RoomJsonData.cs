using System;

[Serializable]
public class RoomJsonData : IData {
    public string id;
    public string share_link;
    public string user;
    public string agora_sid;
    public string agora_channel;
    public string status;

    public string Id => id;

    public string ShareLink => share_link;

    public string Username => user;

    public string Status => status;
}

[Serializable]
public class RoomJsonPutData {
    public string agora_sid;
    public string agora_channel;
    public string status;
}

