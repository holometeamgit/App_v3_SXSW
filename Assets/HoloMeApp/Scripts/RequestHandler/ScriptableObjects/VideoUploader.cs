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

    public string GetStreamToken = "/agora/token/";

    public string UploadPreview = "/stream/{id}/preview/";

}
