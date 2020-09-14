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

    public Action<string> OnStreamStarted;

    TokenAgoraResponse tokenAgoraResponse;
    RequestCloudRecordAcquire requestCloudRecordAcquire;
    RequestCloudRecordResource requestCloudRecordResource;
    StreamStartResponseJsonData streamStartResponseJsonData;
    RequestCloudRecordStop requestCloudRecordStop;

    string streamName;

    public void StartStream(string streamName) {

        this.streamName = streamName;

        //Get token
        // get agora token 
        webRequestHandler.GetRequest(webRequestHandler.ServerURLAuthAPI + videoUploader.GetStreamToken, (x, y) => { AssignToken(x, y); webRequestHandler.LogCallback(x, y); },
            (x, y) => webRequestHandler.ErrorLogCallback(x, "Agora Record Token" + y), accountManager.GetAccessToken().access);

        //Acquire

        //Start cloud record

        //Create stream Valery's server

        //Create stream Agora SDK

        //Stream video
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
        //Acquire
        HelperFunctions.DevLog("ACQUIRE CALLED");
        requestCloudRecordAcquire = new RequestCloudRecordAcquire();
        requestCloudRecordAcquire.AgoraCloudAcquireRequestData = new RequestCloudRecordAcquire.AgoraCloudAcquireRequest() { cname = streamName };
        requestCloudRecordAcquire.OnSuccessAction -= OnAcquireComplete;
        requestCloudRecordAcquire.OnSuccessAction += OnAcquireComplete;
        requestCloudRecordAcquire.OnFailedAction = () => Debug.LogError("Cloud Record Acquire Error");
        agoraRequests.MakePostRequest(requestCloudRecordAcquire, JsonUtility.ToJson(requestCloudRecordAcquire.AgoraCloudAcquireRequestData));
    }

    void OnAcquireComplete() {
        HelperFunctions.DevLog("ACQUIRE COMPLETE  RESOURCE ID = " + requestCloudRecordAcquire.ResponseAcquiredata.resourceId);

        requestCloudRecordResource = new RequestCloudRecordResource();
        requestCloudRecordResource.OnSuccessAction -= OnStartCloudRecordComplete;
        requestCloudRecordResource.OnSuccessAction += OnStartCloudRecordComplete;
        requestCloudRecordResource.OnFailedAction = () => Debug.LogError("Cloud Record Start Error");
        requestCloudRecordResource.AssignResourceId(requestCloudRecordAcquire.ResponseAcquiredata.resourceId);
        requestCloudRecordResource.StartCloudRecordRequestData = new RequestCloudRecordResource.StartCloudRecordRequest();
        requestCloudRecordResource.StartCloudRecordRequestData.uid = requestCloudRecordAcquire.AgoraCloudAcquireRequestData.uid;
        requestCloudRecordResource.StartCloudRecordRequestData.cname = requestCloudRecordAcquire.AgoraCloudAcquireRequestData.cname; //"zed";
        requestCloudRecordResource.StartCloudRecordRequestData.clientRequest.token = tokenAgoraResponse.token;
        agoraRequests.MakePostRequest(requestCloudRecordResource, JsonUtility.ToJson(requestCloudRecordResource.StartCloudRecordRequestData));

        HelperFunctions.DevLog(JsonUtility.ToJson(requestCloudRecordResource.StartCloudRecordRequestData, true));
    }

    void OnStartCloudRecordComplete() {
        HelperFunctions.DevLog($"Cloud recording started: sid = {requestCloudRecordResource.CloudRecordResponseData.sid}");
        CreateStreamSecondaryServer();
    }

    void CreateStreamSecondaryServer() {
        StreamStartJsonData data = new StreamStartJsonData();
        data.agora_sid = requestCloudRecordResource.CloudRecordResponseData.sid;
        data.agora_channel = requestCloudRecordResource.StartCloudRecordRequestData.cname;
        data.file_name_prefix = requestCloudRecordResource.StartCloudRecordRequestData.clientRequest.storageConfig.fileNamePrefix[0];
        //data.title = "";
        //data.description = "";
        HelperFunctions.DevLog(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream);
        webRequestHandler.PostRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream, data, WebRequestHandler.BodyType.JSON, (x, y) => { CreateStreamSecondaryCallback(x, y); webRequestHandler.LogCallback(x, y); }, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    void CreateStreamSecondaryCallback(long code, string data) {
        print("CREATE STREAM IS BACK" + data);
        streamStartResponseJsonData = JsonUtility.FromJson<StreamStartResponseJsonData>(data);
        if (streamStartResponseJsonData != null)
            OnStreamStarted?.Invoke(tokenAgoraResponse.token);
        else {
            Debug.LogError("CREATE STREAM PARSE FAILED");
        }
    }

    public void EndStream() {
        StopCloudRecording();
    }

    void StopCloudRecording() {
        requestCloudRecordStop = new RequestCloudRecordStop();
        requestCloudRecordStop.OnSuccessAction -= StopCloudRecordingCallback;
        requestCloudRecordStop.OnSuccessAction += StopCloudRecordingCallback;
        requestCloudRecordStop.AssignRequestString(requestCloudRecordResource.RequestString, requestCloudRecordResource.CloudRecordResponseData.sid);
        //requestCloudRecordStop.StartCloudRecordRequestData = requestCloudRecordResource.StartCloudRecordRequestData;

        HelperFunctions.DevLog("STOPPING CLOUD RECORDING:" + requestCloudRecordStop.RequestString);
        agoraRequests.MakePostRequest(requestCloudRecordStop, JsonUtility.ToJson(requestCloudRecordResource.StartCloudRecordRequestData));
    }

    void StopCloudRecordingCallback() {

        HelperFunctions.DevLog($"Cloud Recording Stopped: {requestCloudRecordStop.requestCloudRecordStopResponse.serverResponse.uploadingStatus}");

        StopSecondaryServer();
    }

    void StopSecondaryServer() {
        StreamStatusJsonData data = new StreamStatusJsonData();
        data.status = "finished";

        HelperFunctions.DevLog(webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString()));
        webRequestHandler.PatchRequest(webRequestHandler.ServerURLMediaAPI + videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString()), data, WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }
}
