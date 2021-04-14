using System;
using UnityEngine;

public class SecondaryServerCalls : MonoBehaviour {
    [SerializeField]
    VideoUploader videoUploader;

    [SerializeField]
    WebRequestHandler webRequestHandler;

    [SerializeField]
    AgoraRequests agoraRequests;

    [SerializeField]
    AccountManager accountManager;

    public Action<string, string, int> OnStreamStarted;
    string streamName;
    bool isRoom;

    TokenAgoraResponse tokenAgoraResponse;
    RequestCloudRecordAcquire requestCloudRecordAcquire;
    RequestCloudRecordResource requestCloudRecordResource;
    StreamStartResponseJsonData streamStartResponseJsonData;
    RequestCloudRecordStop requestCloudRecordStop;

    public void UploadPreviewImage(byte[] imageData) {
        HelperFunctions.DevLog("UPLOADING PREVIEW IMAGE");
        webRequestHandler.PostRequestMultipart(webRequestHandler.ServerURLMediaAPI + videoUploader.UploadPreview.Replace("{id}", streamStartResponseJsonData.id.ToString()), imageData,
            WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    public void StartStream(string streamName, bool isRoom) {
        this.streamName = streamName;
        this.isRoom = isRoom;
        GetAgoraToken(AssignToken);

        //Get agora token 
        //Acquire
        //Start cloud record
        //Create stream Valery's server
        //Create stream Agora SDK
        //Stream video
        //Stop cloud
        //Stop stream second server
    }

    public void GetAgoraToken(ResponseDelegate callback, string channelName = "") {
        string channelParam = channelName == "" ? channelName : $"?channel={channelName}";
        webRequestHandler.GetRequest(webRequestHandler.ServerURLAuthAPI + videoUploader.GetStreamToken + channelParam, (x, y) => { callback(x, y); webRequestHandler.LogCallback(x, y); },
            (x, y) => webRequestHandler.ErrorLogCallback(x, "Agora Record Token" + y), accountManager.GetAccessToken().access);
    }

    void AssignToken(long code, string data) {

        print("TOKEN IS BACK" + data);
        tokenAgoraResponse = JsonUtility.FromJson<TokenAgoraResponse>(data);
        if (tokenAgoraResponse != null)
            StartAcquire();
        else {
            Debug.LogError("TOKEN PARSE FAILED");
        }
    }

    void StartAcquire() {
        HelperFunctions.DevLog("ACQUIRE CALLED");
        requestCloudRecordAcquire = new RequestCloudRecordAcquire();
        requestCloudRecordAcquire.AgoraCloudAcquireRequestData = new RequestCloudRecordAcquire.AgoraCloudAcquireRequest() { cname = streamName };
        requestCloudRecordAcquire.OnSuccessAction -= OnAcquireComplete;
        requestCloudRecordAcquire.OnSuccessAction += OnAcquireComplete;
        requestCloudRecordAcquire.OnFailedAction = () => Debug.LogError("Cloud Record Acquire Error");
        agoraRequests.MakePostRequest(requestCloudRecordAcquire, JsonUtility.ToJson(requestCloudRecordAcquire.AgoraCloudAcquireRequestData));
    }

    void OnAcquireComplete() {
        HelperFunctions.DevLog("ACQUIRE COMPLETE  RESOURCE ID = " + JsonUtility.ToJson(requestCloudRecordAcquire.ResponseAcquiredata, true));
        requestCloudRecordResource = new RequestCloudRecordResource();
        requestCloudRecordResource.OnSuccessAction -= OnStartCloudRecordComplete;
        requestCloudRecordResource.OnSuccessAction += OnStartCloudRecordComplete;
        requestCloudRecordResource.OnFailedAction = () => Debug.LogError("Cloud Record Start Error");
        requestCloudRecordResource.AssignResourceId(requestCloudRecordAcquire.ResponseAcquiredata.resourceId);
        requestCloudRecordResource.StartCloudRecordRequestData = new RequestCloudRecordResource.StartCloudRecordRequest();
        requestCloudRecordResource.StartCloudRecordRequestData.uid = requestCloudRecordAcquire.AgoraCloudAcquireRequestData.uid;
        requestCloudRecordResource.StartCloudRecordRequestData.cname = requestCloudRecordAcquire.AgoraCloudAcquireRequestData.cname;
        requestCloudRecordResource.StartCloudRecordRequestData.clientRequest.token = tokenAgoraResponse.token;
        agoraRequests.MakePostRequest(requestCloudRecordResource, JsonUtility.ToJson(requestCloudRecordResource.StartCloudRecordRequestData));
        HelperFunctions.DevLog(JsonUtility.ToJson(requestCloudRecordResource.StartCloudRecordRequestData, true));
    }

    void OnStartCloudRecordComplete() {
        HelperFunctions.DevLog($"Cloud recording started: sid = {requestCloudRecordResource.CloudRecordResponseData.sid}");
        if (isRoom)
            CreateRoom();
        else
            CreateStreamSecondaryServer();
    }

    void CreateStreamSecondaryServer() {
        StreamStartJsonData data = new StreamStartJsonData();
        data.agora_sid = requestCloudRecordResource.CloudRecordResponseData.sid;
        data.agora_channel = requestCloudRecordResource.StartCloudRecordRequestData.cname;
        data.file_name_prefix = requestCloudRecordResource.StartCloudRecordRequestData.clientRequest.storageConfig.fileNamePrefix[0];
        data.title = streamName;
        //data.description = "";
        HelperFunctions.DevLog(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream);
        webRequestHandler.PostRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream, data, WebRequestHandler.BodyType.JSON, (x, y) => { CreateStreamSecondaryCallback(x, y); webRequestHandler.LogCallback(x, y); }, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    void CreateRoom() {
        HelperFunctions.DevLog("PUTTING ROOM DATA");
        RoomJsonPutData data = new RoomJsonPutData();
        data.agora_sid = requestCloudRecordResource.CloudRecordResponseData.sid;
        data.agora_channel = requestCloudRecordResource.StartCloudRecordRequestData.cname;
        data.status = StreamJsonData.Data.LIVE_ROOM_STR;
        webRequestHandler.PutRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.PutRoom, data, WebRequestHandler.BodyType.JSON, (x, y) => { CreateStreamSecondaryCallback(x, y); webRequestHandler.LogCallback(x, "Put Room callback: " + y); }, (x, y) => webRequestHandler.ErrorLogCallback(x, "Put Room error callback: " + y), accountManager.GetAccessToken().access);
    }

    void CreateStreamSecondaryCallback(long code, string data) {
        print("CREATE STREAM IS BACK" + data);
        streamStartResponseJsonData = JsonUtility.FromJson<StreamStartResponseJsonData>(data);

        if (streamStartResponseJsonData != null) {
            OnStreamStarted?.Invoke(tokenAgoraResponse.token, tokenAgoraResponse.token, streamStartResponseJsonData.id);
            StreamCallBacks.onLiveStreamCreated?.Invoke(streamStartResponseJsonData.id.ToString());
        } else
            Debug.LogError("CREATE STREAM PARSE FAILED");
    }

    public void EndStream() {
        StopCloudRecording();

        if (isRoom)
            SetRoomToClosed();
        else
            StopSecondaryServer();
    }

    void StopCloudRecording() {
        requestCloudRecordStop = new RequestCloudRecordStop();
        //requestCloudRecordStop.OnSuccessAction -= StopCloudRecordingCallback;
        //requestCloudRecordStop.OnSuccessAction += StopCloudRecordingCallback;
        requestCloudRecordStop.AssignRequestString(requestCloudRecordResource.RequestString, requestCloudRecordResource.CloudRecordResponseData.sid);

        RequestCloudRecordStop.StopCloudRecordRequest payload = new RequestCloudRecordStop.StopCloudRecordRequest();
        payload.uid = requestCloudRecordResource.StartCloudRecordRequestData.uid;
        payload.cname = requestCloudRecordResource.StartCloudRecordRequestData.cname;

        HelperFunctions.DevLog("STOPPING CLOUD RECORDING:" + requestCloudRecordStop.RequestString);
        HelperFunctions.DevLog("PAYLOAD: " + JsonUtility.ToJson(payload, true));
        agoraRequests.MakePostRequest(requestCloudRecordStop, JsonUtility.ToJson(payload));
    }

    //void StopCloudRecordingCallback() {
    //    //HelperFunctions.DevLog($"Cloud Recording Stopped: {requestCloudRecordStop.requestCloudRecordStopResponse.serverResponse.uploadingStatus}");

    //}

    //SetRoomToClosed may be compatible with setting /stream/ status to stop as per Valery's reccomendation leaving as status is currently an empty string rather than "stop"
    void SetRoomToClosed() {
        HelperFunctions.DevLog("PUTTING ROOM CLOSED DATA");
        RoomJsonPutData data = new RoomJsonPutData();
        data.agora_sid = requestCloudRecordResource.CloudRecordResponseData.sid;
        data.agora_channel = requestCloudRecordResource.StartCloudRecordRequestData.cname;
        data.status = string.Empty;
        webRequestHandler.PutRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.PutRoom, data, WebRequestHandler.BodyType.JSON, (x, y) => webRequestHandler.LogCallback(x, "Put Room Closed callback: " + y), (x, y) => webRequestHandler.ErrorLogCallback(x, "Put Room Closed error callback: " + y), accountManager.GetAccessToken().access);
    }

    void StopSecondaryServer() {
        HelperFunctions.DevLog("STOPPING STREAM SECONDARY SERVER");
        StreamStatusJsonData data = new StreamStatusJsonData { status = "stop" };
        HelperFunctions.DevLog(webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString()));
        webRequestHandler.PatchRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString()), data, WebRequestHandler.BodyType.JSON, (x, y) => { webRequestHandler.LogCallback(x, y); HelperFunctions.DevLog("STREAM STOPPED SECONDARY SERVER"); }, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }
}
