using System;

[Serializable]
public class StreamStartJsonData {
    public string agora_sid = "2d26583b814dcc71ce329c9095db5131"; //use this for open stream from customer;
    public string agora_channel = "username"; //use this for open stream from customer;
    public string file_name_prefix = "Beem";
    public string title = "title";
    public string description = "description";
}

[Serializable]
public class StreamStartResponseJsonData {
    public int id;
    public string agora_sid = "2d26583b814dcc71ce329c9095db5131"; //use this for open stream from customer;
    public string agora_channel = "username"; //use this for open stream from customer;
    public string start_date = "start_date";
    public string end_date = "end_date";
    public int duration;
    public string preview_s3_url;
    public string stream_s3_url;
    public string title;
    public string description;
    public string status = "live";
    public string user;
}

[Serializable]
public class TokenAgoraResponse {
    public string token;
}

[Serializable]
public class ThumbnailImageData {
    public string image;
}
