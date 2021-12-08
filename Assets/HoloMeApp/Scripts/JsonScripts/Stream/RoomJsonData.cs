﻿using System;

[Serializable]
public class RoomJsonData {
    public string id;
    public string share_link;
    public string user;
    public string agora_sid;
    public string agora_channel;
    public string status;
}

[Serializable]
public class RoomJsonPutData {
    public string agora_sid;
    public string agora_channel;
    public string status;
}

