using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
/// <summary>
/// GetWebRequester for get Web Request
/// </summary>
public class GetWebRequester : WebRequester {
    /// <summary>
    /// Get webrequest
    /// </summary>
    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> downloadProgress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetRequest(currentUrl, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, downloadProgress: downloadProgress).ContinueWith((taskWebRequestData) => {
            var result = taskWebRequestData.Result;
            (responseDelegate as ResponseDelegate)?.Invoke(result.Code, result.Body);
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for get request
    /// </summary>
    private UnityWebRequest PrepareGetRequest(string url, string headerAccessToken = null) {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new CustomCertificateHandler();

        if (headerAccessToken != null) {
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
        }

        request.timeout = TIMEOUT;

        return request;
    }
}
