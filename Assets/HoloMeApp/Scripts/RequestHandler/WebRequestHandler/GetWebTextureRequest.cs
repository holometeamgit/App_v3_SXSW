using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;

public class GetWebTextureRequest : WebRequester {

    private const int TIMEOUT_TEXTURE_REQUEST = 120000;
    private const int MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST = 240;

    /// <summary>
    /// get texture webrequest
    /// </summary>
    public void GetTextureRequest(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> downloadProgress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetTextureRequest(currentUrl);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate,
            onCancel, downloadProgress: downloadProgress,  maxTimesWait: MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for get texture request text data
    /// </summary>
    private UnityWebRequest PrepareGetTextureRequest(string url) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.downloadHandler = new DownloadHandlerTexture();

        request.timeout = TIMEOUT_TEXTURE_REQUEST;

        return request;
    }
}
