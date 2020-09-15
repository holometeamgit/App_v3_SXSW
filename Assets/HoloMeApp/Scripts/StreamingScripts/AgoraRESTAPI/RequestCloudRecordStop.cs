using System;
using System.Collections.Generic;

public class RequestCloudRecordStop : RestRequest {

    //public RequestCloudRecordResource.StartCloudRecordRequest StartCloudRecordRequestData { private get; set; };
    public RequestCloudRecordStopResponse requestCloudRecordStopResponse;

    /// <summary>
    /// Pass request string from CloudRecordResource
    /// </summary>
    public void AssignRequestString(string cloudRecordStartRequestString, string sid) {
        //v1/apps/{appid}/cloud_recording/resourceid/{resourceid}/sid/{sid}/mode/{mode}/stop
        requestString = cloudRecordStartRequestString;
        requestString = requestString.Replace("/start", "/stop");
        requestString = requestString.Replace("/mode", "/sid/" + sid + "/mode");
    }

    public override void OnSuccess(string result) {
        //result = result.Replace(/:\s * (-?\d +),/ g, ': "$1",'); Consider using regex to convert sliceStartTime to a string
        //requestCloudRecordStopResponse = OnResponseReturned<RequestCloudRecordStopResponse>(result);
        base.OnSuccess(result);
    }

    [Serializable]
    public class StopCloudRecordRequest {
        public string cname;
        public string uid;
        public ClientRequest clientRequest;
    }

    [Serializable]
    public class ClientRequest {
    }

    [Serializable]
    public class RequestCloudRecordStopResponse {
        public string resourceId;
        public string sid;
        public ServerResponse serverResponse = new ServerResponse();
    }

    [Serializable]
    public class FileList {
        public string filename;
        public string trackType;
        public string uid;
        public bool mixedAllUser;
        public bool isPlayable;
        public string sliceStartTime; //This is a problematic value as it's a 14 digit number which isn't easily supported in C# consider converting to a string via regex
    }

    [Serializable]
    public class ServerResponse {
        public string fileListMode;
        public List<FileList> fileList = new List<FileList>();
        public string uploadingStatus;
    }
}
