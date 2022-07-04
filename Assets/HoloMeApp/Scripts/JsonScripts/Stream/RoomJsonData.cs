using System;

[Serializable]
public class RoomJsonData : IData {
    public string id;
    public string share_link;
    public string user;
    public string agora_sid;
    public string agora_channel;
    public string status;

    public string GetId => id;

    public string GetShareLink => share_link;

    public string GetUsername => user;

    public string GetStatus => status;
}

[Serializable]
public class RoomJsonPutData {
    public string agora_sid;
    public string agora_channel;
    public string status;
}

