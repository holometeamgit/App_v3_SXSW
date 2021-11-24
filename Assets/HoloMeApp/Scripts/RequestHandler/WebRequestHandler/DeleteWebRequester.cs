using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;

public class DeleteWebRequester : WebRequester
{
    /// <summary>
    /// Delete webrequest
    /// </summary>
    public void DeleteRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, ActionWrapper onCancel = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareDeleteRequest(currentUrl, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for delete request text data
    /// </summary>
    private UnityWebRequest PrepareDeleteRequest(string url, string headerAccessToken = null) {
        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.certificateHandler = new CustomCertificateHandler();
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        request.timeout = TIMEOUT;

        return request;
    }
}
