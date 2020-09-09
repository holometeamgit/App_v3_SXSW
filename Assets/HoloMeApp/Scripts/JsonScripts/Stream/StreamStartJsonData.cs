using System;
using System.Collections.Generic;

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

#region Agora Recording

#region request classes
[Serializable]
public class TranscodingConfig {
    public int height { get; set; }
    public int width { get; set; }
    public int bitrate { get; set; }
    public int fps { get; set; }
    public int mixedVideoLayout { get; set; }
    public string backgroundColor { get; set; }
}

[Serializable]
public class RecordingConfig {
    public int maxIdleTime { get; set; }
    public int streamTypes { get; set; }
    public int channelType { get; set; }
    public int videoStreamType { get; set; }
    public TranscodingConfig transcodingConfig { get; set; }
    public List<string> subscribeVideoUids { get; set; }
    public List<string> subscribeAudioUids { get; set; }
    public int subscribeUidGroup { get; set; }
}

[Serializable]
public class RecordingFileConfig {
    public List<string> avFileType { get; set; }
}

[Serializable]
public class StorageConfig {
    public string accessKey { get; set; } = "AKIA2VJEQIYYQAVB4T67";
    public int region { get; set; } = 5;
    public string bucket { get; set; } = "dev.agora";
    public string secretKey { get; set; } = "tFAwjZ1LNUrfXq7eoBlBC33j7qQWjn3KMdiwCSyO";
    public int vendor { get; set; }
    public List<string> fileNamePrefix { get; set; } = new List<string>() { "Beem_Recording_" };
}

[Serializable]
public class ClientRequest {
    public string token { get; set; }
    public RecordingConfig recordingConfig { get; set; }
    public RecordingFileConfig recordingFileConfig { get; set; }
    public StorageConfig storageConfig { get; set; }
}

[Serializable]
public class StartCloudRecordRequest {
    public string cname { get; set; }
    public string uid { get; set; }
    public ClientRequest clientRequest { get; set; }
}

[Serializable]
public class CloudRecordResponse {
    public string sid { get; set; }
    public string resourceId { get; set; }
}

#endregion

#region Acquire functiona
[Serializable]
public class AgoraCloudAcquireRequest {
    public string cname { get; set; }
    public string uid { get; set; }
    public ClientRequestAcquire clientRequest { get; set; }
}

[Serializable]
public class ClientRequestAcquire {
    public int resourceExpiredHour { get; set; }
}

[Serializable]
public class ResponseAcquire {
    public string resourceId { get; set; }
}
#endregion

#endregion