using System;
using System.Collections.Generic;

public class RequestCloudRecordResource : RestRequest {

    public StartCloudRecordRequest StartCloudRecordRequestData { get; set; }
    public CloudRecordResponse CloudRecordResponseData { get; private set; }

    //https://docs.agora.io/en/cloud-recording/restfulapi/#/Cloud%20Recording/start

    const string ModeIndividual = "individual";
    const string ModeMix = "mix";

    public void AssignResourceId(string id) {
        requestString = $"/v1/apps/{AgoraController.AppId}/cloud_recording/resourceid/{id}/mode/{ModeIndividual}/start";
    }

    public override void OnSuccess(string result) {
        CloudRecordResponseData = OnResponseReturned<CloudRecordResponse>(result);
        base.OnSuccess(result);
    }

    [Serializable]
    public class StartCloudRecordRequest {
        public string cname;
        public string uid;
        public ClientRequest clientRequest = new ClientRequest();
    }

    [Serializable]
    public class ClientRequest {
        public string token;
        public RecordingConfig recordingConfig = new RecordingConfig();
        public RecordingFileConfig recordingFileConfig = new RecordingFileConfig();
        public StorageConfig storageConfig = new StorageConfig();
    }

    [Serializable]
    public class TranscodingConfig {
        public int height = 640;
        public int width = 360;
        public int bitrate = 500;
        public int fps = 15;
        public int mixedVideoLayout = 1;
        public string backgroundColor = "#000000";
    }

    [Serializable]
    public class RecordingConfig {
        public int maxIdleTime = 30;
        public int streamTypes = 2;
        public int channelType = 0;
        public int videoStreamType = 0;
        //public TranscodingConfig transcodingConfig = new TranscodingConfig();
        public List<string> subscribeVideoUids = new List<string> { "#allstream#" };
        public List<string> subscribeAudioUids = new List<string> { "#allstream#" };
        //public int subscribeUidGroup = 0;
    }

    [Serializable]
    public class RecordingFileConfig {
        public List<string> avFileType = new List<string> { "hls" };
    }

    [Serializable]
    public class StorageConfig {
        public string accessKey = "AKIA2VJEQIYYQAVB4T67";
        public int region = 5;
        public string bucket = "dev.agora";
        public string secretKey = "tFAwjZ1LNUrfXq7eoBlBC33j7qQWjn3KMdiwCSyO";
        public int vendor = 1;
        public List<string> fileNamePrefix = new List<string>() { "mobile_app" };
    }

    [Serializable]
    public class CloudRecordResponse {
        public string sid;
        public string resourceId;
    }
}
