using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
/// <summary>
/// PatchWebRequester for patch  Web Request
/// </summary>
public class PatchWebRequester : WebRequester {
    /// <summary>
    /// Patch webrequest
    /// </summary>
    public void PatchRequest<T>(string url, T body, WebRequestBodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, ActionWrapper onCancel = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            T currentBody = body;
            WebRequestBodyType currentBodyType = bodyType;
            return PreparePatchRequest(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel).ContinueWith((taskWebRequestData) => {
            var result = taskWebRequestData.Result;
            (responseDelegate as ResponseDelegate)?.Invoke(result.Code, result.Body);
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for Patch request text data
    /// </summary>
    private UnityWebRequest PreparePatchRequest<T>(string url, T body, WebRequestBodyType bodyType, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PATCH");
        request.certificateHandler = new CustomCertificateHandler();

        switch (bodyType) {
            default: //only Json at this moment
                string bodyString = JsonUtility.ToJson(body);
                bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                break;
        }

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        request.timeout = TIMEOUT;

        return request;
    }
}
