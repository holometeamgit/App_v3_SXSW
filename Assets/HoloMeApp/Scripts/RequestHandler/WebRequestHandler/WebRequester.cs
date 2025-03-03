using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
/// <summary>
/// Async web request class
/// </summary>
public class WebRequester 
{
    protected const int TIMEOUT = 5000;
    protected const int MAX_TIMES_BEFORE_STOP_REQUEST = 20;
    protected const int REQUEST_CHECK_COOLDOWN = 250;

    public class Result {
        public long Code;
        public string Body;
        public Texture2D Texture;

        public Result(long code, string body, Texture2D texture = null) {
            Code = code;
            Body = body;
            Texture = texture;
        }
    }

    #region Async web request
    /// <summary>
    /// Async WebRequest with Retry 
    /// </summary>
    protected async Task<Result> WebRequestWithRetryAsync<T>(Func<UnityWebRequest> createWebRequest,
        T responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        ActionWrapper onCancel = null, Action<float> downloadProgress = null, Action<float> uploadProgress = null, int maxTimesWait = MAX_TIMES_BEFORE_STOP_REQUEST) {

        UnityWebRequest request = null;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        if (onCancel != null) {
            onCancel.AddListener(cancellationTokenSource.Cancel);
        }
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        if (onCancel != null && onCancel.WasCalled) {
            HelperFunctions.DevLog("onCancel.WasCalled" + onCancel.WasCalled);
            cancellationTokenSource.Cancel();
        }

        try {
            request = createWebRequest?.Invoke();

            Task requestTask = UnityWebRequestAsync(request, cancellationToken, maxTimesWait, downloadProgress: downloadProgress, uploadProgress: uploadProgress);
            await RetryAsyncHelpe.RetryOnExceptionAsync<UnityWebRequestServerConnectionException>(async () => { await requestTask; });

            if (responseDelegate is ResponseDelegate) {
                return new Result(request.responseCode, request.downloadHandler.text);
            } else if (responseDelegate is ResponseTextureDelegate) {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                return new Result(request.responseCode, request.downloadHandler.text, texture);
            }

        } catch (UnityWebRequestException uwrException) {
            HelperFunctions.DevLogError("UnityWebRequestException: " + request.uri + " " + uwrException.Code + " " + uwrException.Message);
            errorTypeDelegate?.Invoke(uwrException.Code, uwrException.Message);
        } catch (UnityWebRequestServerConnectionException uwrServerConnectionException) {
            HelperFunctions.DevLogError("UnityWebRequestServerConnectionException: " + request.uri + " " + uwrServerConnectionException.Code + " " + uwrServerConnectionException.Message);
            errorTypeDelegate?.Invoke(uwrServerConnectionException.Code, uwrServerConnectionException.Message);
        } catch (Exception exception) {
            HelperFunctions.DevLogError("Exception: WebRequestError  " + request.uri + " " + exception.Message);
            errorTypeDelegate?.Invoke(500, "Failed to connect to server: " + exception.Message);
        } finally {
            if (onCancel != null) {
                onCancel.RemoveListener(cancellationTokenSource.Cancel);
            }
            cancellationTokenSource.Dispose();
            request?.Dispose();
            UnityWebRequest.ClearCookieCache();
        }
        return default;
    }

    /// <summary>
    /// Unity Asynchronous request
    /// if nothing happens waiting time = REQUEST_CHECK_COOLDOWN * MAX_COUNT_STOPPED_STEPS_REQUEST and then timeout exception
    /// </summary>
    private async Task UnityWebRequestAsync(UnityWebRequest request, CancellationToken cancellationToken, int maxTimesWait,
        Action<float> downloadProgress = null, Action<float> uploadProgress = null) {
        int countStoppedSteps = 0;
        float prevDownloadProgressState = request.downloadProgress;
        float prevUploadProgressState = request.uploadProgress;

        request.SendWebRequest();

        await Task.Delay(REQUEST_CHECK_COOLDOWN);

        //awaiting
        while (!request.isDone) {
            downloadProgress?.Invoke(request.downloadProgress);
            uploadProgress?.Invoke(request.uploadProgress);
            //check cancel
            if (cancellationToken.IsCancellationRequested) {
                request.Abort();
                cancellationToken.ThrowIfCancellationRequested();
                //check timeout
            } else if (IsServerWaitingTimeout(ref countStoppedSteps, ref prevDownloadProgressState, ref prevUploadProgressState, request, maxTimesWait)) {
                request.Abort();
                throw new UnityWebRequestServerConnectionException(504, "Gateway Timeout");
            }

            await Task.Delay(REQUEST_CHECK_COOLDOWN);
        }

        if (request.isNetworkError || request.isHttpError) {
            if (request.responseCode >= 500) {
                throw new UnityWebRequestServerConnectionException(request.responseCode, request.downloadHandler.text);
            } else {
                throw new UnityWebRequestException(request.responseCode, request.downloadHandler.text);
            }
        }

        downloadProgress?.Invoke(request.downloadProgress);
        uploadProgress?.Invoke(request.uploadProgress);
    }

    /// <summary>
    /// check server timeout
    /// </summary>
    private bool IsServerWaitingTimeout(ref int countStoppedSteps, ref float prevDownloadProgressState, ref float prevUploadProgressState, UnityWebRequest request, int maxTimesWait) {
        if (prevDownloadProgressState == request.downloadProgress && prevUploadProgressState == request.uploadProgress) {
            countStoppedSteps++;
        } else {
            prevDownloadProgressState = request.downloadProgress;
            prevUploadProgressState = request.uploadProgress;
            countStoppedSteps = 0;
        }

        return countStoppedSteps >= maxTimesWait;
    }
    #endregion
}
