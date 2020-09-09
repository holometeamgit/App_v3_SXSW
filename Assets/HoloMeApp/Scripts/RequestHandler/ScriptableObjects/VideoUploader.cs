using UnityEngine;

[HelpURL("https://devholo.me/docs/auth/?urls.primaryName=Video%20Uploader#/")]
[CreateAssetMenu(fileName = "VideoUploaderAPI", menuName = "Data/API/VideoUploaderAPI")]
public class VideoUploader : ScriptableObject {
    [Header("Stream")]
    [Tooltip("get raw videos and processing statuses")]
    public string Stream = "/stream/";

    [Header("Change Status")]
    [Tooltip("Change Status, used to do things like end stream")]
    public string StreamStatus = "/stream/{id}/status/";

    public string AgoraAcquireResourceID = $"/v1/apps/{AgoraController.AppId}/cloud_recording/acquire";

    [Header("Start Cloud recording")]
    [Tooltip("For Agora")]
    public string AgoraStartCloudRecording = "/v1/apps/{appid}/cloud_recording/resourceid/{resourceid}/mode/{mode}/start";

    public string AgoraStopCloudRecording = "/v1/apps/{appid}/cloud_recording/resourceid/{resourceid}/sid/{sid}/mode/{mode}/stop";

}
