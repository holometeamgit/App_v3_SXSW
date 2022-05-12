using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// GetWebTextureRequest for get Texture Web Request
/// </summary>
public class GetWebTextureRequest : WebRequester {

    private const int TIMEOUT_TEXTURE_REQUEST = 120000;
    private const int MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST = 240;

    /// <summary>
    /// get texture webrequest
    /// </summary>
    public void GetTextureRequest(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> downloadProgress = null, bool nonreadable = true) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetTextureRequest(currentUrl, nonreadable);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate,
            onCancel, downloadProgress: downloadProgress, maxTimesWait: MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST).ContinueWith((taskWebRequestData) => {
                var result = taskWebRequestData.Result;
                (responseDelegate as ResponseTextureDelegate)?.Invoke(result.Code, result.Body, result.Texture);
            }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for get texture request text data
    /// </summary>
    private UnityWebRequest PrepareGetTextureRequest(string url, bool nonreadable) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, nonreadable);
        request.downloadHandler = new DownloadHandlerTexture();

        request.timeout = TIMEOUT_TEXTURE_REQUEST;

        return request;
    }
}
