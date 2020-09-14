using System;
using System.Collections.Generic;

public class RequestCloudRecordStop : RestRequest {

    //public RequestCloudRecordResource.StartCloudRecordRequest StartCloudRecordRequestData { private get; set; };
    public RequestCloudRecordStopResponse requestCloudRecordStopResponse;

    /// <summary>
    /// Pass request string from CloudRecordResource
    /// </summary>
    public void AssignRequestString(string cloudRecordStartRequestString, string sid) {
        ///v1/apps/{appid}/cloud_recording/resourceid/{resourceid}/sid/{sid}/mode/{mode}/stop
        requestString = cloudRecordStartRequestString.Replace("/start", "/stop");
        requestString = cloudRecordStartRequestString.Replace("/mode", "/sid/" + sid + "/mode");
    }

    public override void OnSuccess(string result) {
        requestCloudRecordStopResponse = OnResponseReturned<RequestCloudRecordStopResponse>(result);
        base.OnSuccess(result);
    }

    [Serializable]
    public class RequestCloudRecordStopResponse {
        public string resourceId;
        public string sid;
        public ServerResponse serverResponse;
    }

    [Serializable]
    public class FileList {
        public string filename;
        public string trackType;
        public string uid;
        public bool mixedAllUser;
        public bool isPlayable;
        public object sliceStartTime;
    }

    [Serializable]
    public class ServerResponse {
        public string fileListMode;
        public List<FileList> fileList;
        public string uploadingStatus;
    }
}
