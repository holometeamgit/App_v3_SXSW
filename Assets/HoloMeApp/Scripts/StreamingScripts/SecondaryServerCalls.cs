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

    string streamName;

    public void StartStream(string streamName, uint uid) {

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
        requestCloudRecordAcquire.OnSuccessAction += OnAcquireComplete;
        requestCloudRecordAcquire.OnFailedAction = () => Debug.LogError("Cloud Record Acquire Error");
        agoraRequests.MakePostRequest(requestCloudRecordAcquire, JsonUtility.ToJson(requestCloudRecordAcquire.AgoraCloudAcquireRequestData));
    }


    void OnAcquireComplete() {
        HelperFunctions.DevLog("ACQUIRE COMPLETE ID = " + requestCloudRecordAcquire.ResponseAcquiredata.resourceId);

        requestCloudRecordResource = new RequestCloudRecordResource();
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
        //data.title = "";
        //data.description = "";
        webRequestHandler.PostRequest(webRequestHandler.ServerURLAuthAPI + videoUploader.Stream, data, WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
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
        StopStream();
    }

    void StopStream() {
        StreamStatusJsonData data = new StreamStatusJsonData();
        data.status = "finished";
        webRequestHandler.PatchRequest(webRequestHandler.ServerURLAuthAPI + (videoUploader.StreamStatus.Replace("{id}", streamStartResponseJsonData.id.ToString())), data, WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    void StopCloudRecording() {

    }

}
