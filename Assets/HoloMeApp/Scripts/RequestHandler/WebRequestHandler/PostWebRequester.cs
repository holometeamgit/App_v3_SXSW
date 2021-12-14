using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
/// <summary>
/// PostWebRequester for Post Web Request
/// </summary>
public class PostWebRequester : WebRequester {

    /// <summary>
    /// post webrequest
    /// </summary>
    public void PostRequest<T>(string url, T body, WebRequestBodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            T currentBody = body;
            WebRequestBodyType currentBodyType = bodyType;
            return PreparePostRequest<T>(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel: onCancel, uploadProgress: uploadProgress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for post request text data
    /// </summary>
    private UnityWebRequest PreparePostRequest<T>(string url, T body, WebRequestBodyType bodyType, string headerAccessToken = null) {

        UnityWebRequest request;

        switch (bodyType) {
            case WebRequestBodyType.XWWWFormUrlEncoded:
                WWWForm form = new WWWForm();
                Type listType = typeof(T);
                if (listType == typeof(Dictionary<string, string>)) {
                    Dictionary<string, string> fields = body as Dictionary<string, string>;
                    foreach (var field in fields) {
                        form.AddField(field.Key, field.Value);
                    }
                } else if (listType == typeof(Dictionary<string, byte[]>)) {
                    Dictionary<string, byte[]> fields = body as Dictionary<string, byte[]>;
                    foreach (var field in fields) {
                        form.AddBinaryData(field.Key, field.Value);
                    }
                }
                request = UnityWebRequest.Post(url, form);

                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                break;
            case WebRequestBodyType.JSON: //only Json at this moment
                string bodyString = JsonUtility.ToJson(body);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                request = new UnityWebRequest(url, "POST");
                request.SetRequestHeader("Content-Type", "application/json");
                request.uploadHandler = new UploadHandlerRaw(data: bodyRaw);
                break;
            case WebRequestBodyType.None:
            default:
                request = new UnityWebRequest(url, "POST");
                break;
        }

        request.certificateHandler = new CustomCertificateHandler();
        request.downloadHandler = new DownloadHandlerBuffer();

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        request.timeout = TIMEOUT;

        return request;

    }
}
