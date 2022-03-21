using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using Zenject;

public class SecondaryServerCalls : MonoBehaviour {
    [SerializeField]
    VideoUploader videoUploader;

    [SerializeField]
    AgoraRequests agoraRequests;

    private WebRequestHandler _webRequestHandler;

    public Action<string, string, int> OnStreamStarted;
    string streamName;
    bool isRoom;

    TokenAgoraResponse tokenAgoraResponse;
    RequestCloudRecordAcquire requestCloudRecordAcquire;
    RequestCloudRecordResource requestCloudRecordResource;
    StreamStartResponseJsonData streamStartResponseJsonData;
    RequestCloudRecordStop requestCloudRecordStop;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    public void UploadPreviewImage(byte[] imageData) {
        HelperFunctions.DevLog("UPLOADING PREVIEW IMAGE");

        Dictionary<string, MultipartRequestBinaryData> formData = new Dictionary<string, MultipartRequestBinaryData>();

        formData.Add("image", new MultipartRequestBinaryData("image", imageData, "VideoThumbnail.png"));

        _webRequestHandler.PostMultipart(_webRequestHandler.ServerURLMediaAPI + videoUploader.UploadPreview.Replace("{id}", streamStartResponseJsonData.id.ToString()),
            formData,
            _webRequestHandler.LogCallback, _webRequestHandler.ErrorLogCallback, needHeaderAccessToken: true);
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
        _webRequestHandler.Get(_webRequestHandler.ServerURLAuthAPI + videoUploader.GetStreamToken + channelParam, (x, y) => { callback(x, y); _webRequestHandler.LogCallback(x, y); },
            (x, y) => _webRequestHandler.ErrorLogCallback(x, "Agora Record Token" + y), needHeaderAccessToken: true);
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
        HelperFunctions.DevLog(_webRequestHandler.ServerURLMediaAPI + videoUploader.Stream);
        _webRequestHandler.Post(_webRequestHandler.ServerURLMediaAPI + videoUploader.Stream,
            data,
            WebRequestBodyType.JSON, (x, y) => { CreateStreamSecondaryCallback(x, y); _webRequestHandler.LogCallback(x, y); },
            _webRequestHandler.ErrorLogCallback, needHeaderAccessToken: true);
    }

    void CreateRoom() {
        HelperFunctions.DevLog("PUTTING ROOM DATA");
        RoomJsonPutData data = new RoomJsonPutData();
        data.agora_sid = requestCloudRecordResource.CloudRecordResponseData.sid;
        data.agora_channel = requestCloudRecordResource.StartCloudRecordRequestData.cname;
        data.status = StreamJsonData.Data.LIVE_ROOM_STR;
        _webRequestHandler.Put(_webRequestHandler.ServerURLMediaAPI + videoUploader.PutRoom,
            data,
            WebRequestBodyType.JSON,
            (x, y) => { CreateStreamSecondaryCallback(x, y); _webRequestHandler.LogCallback(x, "Put Room callback: " + y); },
            (x, y) => _webRequestHandler.ErrorLogCallback(x, "Put Room error callback: " + y), needHeaderAccessToken: true);
    }

    void CreateStreamSecondaryCallback(long code, string data) {
        print("CREATE STREAM IS BACK" + data);
        streamStartResponseJsonData = JsonUtility.FromJson<StreamStartResponseJsonData>(data);

        if (streamStartResponseJsonData != null) {

            if (!isRoom) {
                SetStreamStatusToLive();
            }

            OnStreamStarted?.Invoke(tokenAgoraResponse.token, tokenAgoraResponse.token, streamStartResponseJsonData.id);
            StreamCallBacks.onLiveStreamCreated?.Invoke(streamStartResponseJsonData);
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
        _webRequestHandler.Put(_webRequestHandler.ServerURLMediaAPI + videoUploader.PutRoom,
            data,
            WebRequestBodyType.JSON, (x, y) => _webRequestHandler.LogCallback(x, "Put Room Closed callback: " + y),
            (x, y) => _webRequestHandler.ErrorLogCallback(x, "Put Room Closed error callback: " + y), needHeaderAccessToken: true);
    }

    void StopSecondaryServer() {
        HelperFunctions.DevLog("STOPPING STREAM SECONDARY SERVER");
        StreamStatusJsonData data = new StreamStatusJsonData { status = StreamJsonData.Data.STOP_STR };
        HelperFunctions.DevLog(_webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString()));
        StreamStatusChange(data);
    }

    void SetStreamStatusToLive() {
        StreamStatusJsonData data = new StreamStatusJsonData { status = StreamJsonData.Data.LIVE_STR };
        StreamStatusChange(data);
    }

    void StreamStatusChange(StreamStatusJsonData data) {
        HelperFunctions.DevLog($"Stream status changed " + data.status);
        _webRequestHandler.PatchRequest(_webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}",
            streamStartResponseJsonData.id.ToString()), data, WebRequestBodyType.JSON,
            (x, y) => { _webRequestHandler.LogCallback(x, y); HelperFunctions.DevLog("STREAM STOPPED SECONDARY SERVER"); },
            _webRequestHandler.ErrorLogCallback, needHeaderAccessToken: true);
        StreamCallBacks.onLiveStreamFinished?.Invoke();
    }
}
