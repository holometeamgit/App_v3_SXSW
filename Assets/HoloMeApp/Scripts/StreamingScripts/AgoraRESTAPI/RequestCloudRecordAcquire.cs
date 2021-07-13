using System;

public class RequestCloudRecordAcquire : RestRequest {

    public const string CLOUD_RECORD_UID = "9999";
    public AgoraCloudAcquireRequest AgoraCloudAcquireRequestData { get; set; }
    public ResponseAcquire ResponseAcquiredata { get; private set; }

    public RequestCloudRecordAcquire() {
        requestString = $"/v1/apps/{AgoraController.AppId}/cloud_recording/acquire";
    }

    public override void OnSuccess(string result) {
        //UnityEngine.Debug.Log("ACQUIRE RESULT RAW = " + result);
        ResponseAcquiredata = OnResponseReturned<ResponseAcquire>(result);
        base.OnSuccess(result);
    }

    #region Acquire functions
    [Serializable]
    public class AgoraCloudAcquireRequest {
        public string cname;
        public string uid = CLOUD_RECORD_UID;
        public ClientRequestAcquire clientRequest = new ClientRequestAcquire();
    }

    [Serializable]
    public class ClientRequestAcquire {
        public int resourceExpiredHour = 24;
    }

    [Serializable]
    public class ResponseAcquire {
        public string resourceId;
    }
    #endregion
}
