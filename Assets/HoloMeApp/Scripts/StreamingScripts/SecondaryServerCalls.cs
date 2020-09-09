using UnityEngine;
using UnityEngine.Events;

public class SecondaryServerCalls : MonoBehaviour {
    [SerializeField]
    VideoUploader videoUploader;

    [SerializeField]
    WebRequestHandler webRequestHandler;

    [SerializeField]
    AgoraRequests agoraRequests;

    [SerializeField]
    AccountManager accountManager;

    [SerializeField]
    UserWebManager userWebManager;

    //[HideInInspector]
    //public UnityEvent OnStreamStarted;

    public void StartStream(string streamName, uint uid) {
        print("ACQUIRE CALLED 1");
        AgoraCloudAcquireRequest acquireData = new AgoraCloudAcquireRequest() { cname = streamName, uid = uid.ToString() };

        webRequestHandler.PostRequest(videoUploader.AgoraAcquireResourceID, acquireData, WebRequestHandler.BodyType.JSON, (x, y) => { OnAcquireComplete(streamName, uid, y); webRequestHandler.LogCallback(x, y); },
            webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);

        print("ACQUIRE CALLED 2");

        StreamStartJsonData data = new StreamStartJsonData();
        data.agora_channel = userWebManager.GetUsername();
        data.agora_sid = "";

        //webRequestHandler.PostRequest(GetStartStreamURL(), data, WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    void OnAcquireComplete(string streamName, uint uid, string resultData) {
        print("ACQUIRE COMPLETE");
        var result = JsonUtility.FromJson<ResponseAcquire>(resultData);

        StartCloudRecordRequest startCloudRecordRequestData = new StartCloudRecordRequest();
        startCloudRecordRequestData.cname = streamName;
        startCloudRecordRequestData.uid = uid.ToString();


        //webRequestHandler.PostRequest(, startCloudRecordRequestData, WebRequestHandler.BodyType.JSON, (x, y) => { OnStartRecordComplete(y); webRequestHandler.LogCallback(x, y); },
        //    webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);

    }

    void OnStartRecordComplete(string data) {

    }

    public void EndStream() {
        StreamStatusJsonData data = new StreamStatusJsonData();
        data.status = "finished";
        webRequestHandler.PatchRequest(GetServerStatusURL(), data, WebRequestHandler.BodyType.JSON, webRequestHandler.LogCallback, webRequestHandler.ErrorLogCallback, accountManager.GetAccessToken().access);
    }

    private string GetStartStreamURL() {
        return webRequestHandler.ServerURLAuthAPI + videoUploader.Stream;
    }

    private string GetServerStatusURL() {
        return webRequestHandler.ServerURLAuthAPI + videoUploader.StreamStatus;
    }

}
