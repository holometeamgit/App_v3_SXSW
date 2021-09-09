using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public class WebRequestHandler : MonoBehaviour {

    public enum BodyType {
        None,
        JSON,
        XWWWFormUrlEncoded
    }

    public string ServerURLAuthAPI { get { return serverURLAPI.ServerURLAuthAPI; } private set { } }
    public string ServerURLMediaAPI { get { return serverURLAPI.ServerURLMediaAPI; } private set { } }
    public string ServerProvidersAPI { get { return serverURLAPI.ServerProvidersAPI; } private set { } }

    public string FirebaseAddUserStaticLink { get { return serverURLAPI.FirebaseAddUserStaticLink; } private set { } }

    [SerializeField] ServerURLAPIScriptableObject serverURLAPI;
    [SerializeField] AccountManager accountManager;

    private const int REQUEST_CHECK_COOLDOWN = 250;
    private const int MAX_TIMES_BEFORE_STOP_REQUEST = 20;
    private const int MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST = 240;

    private const int TIMEOUT = 5000;
    private const int TIMEOUT_TEXTURE_REQUEST = 120000;

    public void LogCallback(long code, string body) {
        HelperFunctions.DevLog($"Code {code} Message {body}");
    }

    public void ErrorLogCallback(long code, string body) {
        HelperFunctions.DevLogError($"An Error Occurred! Code {code} Message {body}");
    }

    public void Get(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        bool needHeaderAccessToken = true, Action onCancel = null, Action<float> progress = null) {
        GetRequest(url, responseDelegate, errorTypeDelegate,
            needHeaderAccessToken ? accountManager.GetAccessToken().access : null,
            onCancel, progress);
    }


    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetRequest(currentUrl, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    public void Post<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        bool needHeaderAccessToken = true, Action onCancel = null, Action<float> progress = null) {

        PostRequest(url, body, bodyType, responseDelegate, errorTypeDelegate,
            needHeaderAccessToken ? accountManager.GetAccessToken().access : null,
            onCancel, progress);
    }

        public void PostRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            T currentBody = body;
            BodyType currentBodyType = bodyType;
            return PreparePostRequest<T>(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel: onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Delete webrequest
    /// </summary>
    public void Delete(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        bool needHeaderAccessToken = true, Action onCancel = null, Action<float> progress = null) {
        DeleteRequest(url, responseDelegate, errorTypeDelegate,
            needHeaderAccessToken ? accountManager.GetAccessToken().access : null,
            onCancel, progress);
    }

    public void DeleteRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareDeleteRequest(currentUrl, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    public void PutRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            T currentBody = body;
            BodyType currentBodyType = bodyType;
            return PreparePutRequest(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }


    public void PostRequestMultipart(string url, byte[] body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            byte[] currentBody = body;
            BodyType currentBodyType = bodyType;
            return PreparePostMultipartRequest(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    public void PatchRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            T currentBody = body;
            BodyType currentBodyType = bodyType;
            return PreparePatchRequest(currentUrl, currentBody, currentBodyType, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    public void GetTextureRequest(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
    string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {
        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetTextureRequest(currentUrl);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress, MAX_TIMES_BEFORE_STOP_TEXTURE_REQUEST).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for get request text data
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

    /// <summary>
    /// Prepare UnityWebRequest for post request text data
    /// </summary>
    private UnityWebRequest PreparePostRequest<T>(string url, T body, BodyType bodyType, string headerAccessToken = null) {

        UnityWebRequest request;

        switch (bodyType) {
            case BodyType.XWWWFormUrlEncoded:
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
            case BodyType.JSON: //only Json at this moment
                string bodyString = JsonUtility.ToJson(body);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                request = new UnityWebRequest(url, "POST");
                request.SetRequestHeader("Content-Type", "application/json");
                request.uploadHandler = new UploadHandlerRaw(data: bodyRaw);
                break;
            case BodyType.None:
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

    /// <summary>
    /// Prepare UnityWebRequest for put request text data
    /// </summary>
    private UnityWebRequest PreparePutRequest<T>(string url, T body, BodyType bodyType, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PUT");
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

    /// <summary>
    /// Prepare UnityWebRequest for post Multipart request text data
    /// </summary>
    private UnityWebRequest PreparePostMultipartRequest(string url, byte[] myData, BodyType bodyType, string headerAccessToken = null) {
        // read a file and add it to the form
        List<IMultipartFormSection> form = new List<IMultipartFormSection> { new MultipartFormFileSection("image", myData, "VideoThumbnail.png", "image") };
        // generate a boundary then convert the form to byte[]
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
        // my termination string consisting of CRLF--{boundary}--
        byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
        // Make my complete body from the two byte arrays
        byte[] body = new byte[formSections.Length + terminate.Length];
        Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
        Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);
        // Set the content type - NO QUOTES around the boundary
        string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));
        // Make my request object and add the raw body. Set anything else you need here
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.certificateHandler = new CustomCertificateHandler();
        UploadHandler uploader = new UploadHandlerRaw(body);
        uploader.contentType = contentType;
        request.uploadHandler = uploader;
        request.downloadHandler = new DownloadHandlerBuffer();

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        request.timeout = TIMEOUT;

        return request;
    }

    /// <summary>
    /// Prepare UnityWebRequest for Patch request text data
    /// </summary>
    private UnityWebRequest PreparePatchRequest<T>(string url, T body, BodyType bodyType, string headerAccessToken = null) {
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

    /// <summary>
    /// Prepare UnityWebRequest for get texture request text data
    /// </summary>
    private UnityWebRequest PrepareGetTextureRequest(string url) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.downloadHandler = new DownloadHandlerTexture();

        request.timeout = TIMEOUT_TEXTURE_REQUEST;

        return request;
    }

    #region Async web request
    /// <summary>
    /// Async WebRequest with Retry 
    /// </summary>
    private async Task WebRequestWithRetryAsync<T>(Func<UnityWebRequest> createWebRequest,
        T responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        Action onCancel = null, Action<float> progress = null, int maxTimesWait = MAX_TIMES_BEFORE_STOP_REQUEST) {

        UnityWebRequest request = null;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        if (onCancel != null) {
            onCancel += cancellationTokenSource.Cancel;
        }
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        try {
            request = createWebRequest?.Invoke();
            Task requestTask = UnityWebRequestAsync(request, cancellationToken, progress, maxTimesWait);
            await RetryAsyncHelpe.RetryOnExceptionAsync<UnityWebRequestServerConnectionException>(async () => { await requestTask; });

            if (responseDelegate is ResponseDelegate) {
                (responseDelegate as ResponseDelegate)?.Invoke(request.responseCode, request.downloadHandler.text);
            } else if (responseDelegate is ResponseTextureDelegate) {
                Texture texture = DownloadHandlerTexture.GetContent(request);
                (responseDelegate as ResponseTextureDelegate)?.Invoke(request.responseCode,
                    request.downloadHandler.text, ((DownloadHandlerTexture)request.downloadHandler).texture);
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
                onCancel -= cancellationTokenSource.Cancel;
            }
            cancellationTokenSource.Dispose();
            request?.Dispose();
            UnityWebRequest.ClearCookieCache();
        }
    }

    /// <summary>
    /// Unity Asynchronous request
    /// if nothing happens waiting time = REQUEST_CHECK_COOLDOWN * MAX_COUNT_STOPPED_STEPS_REQUEST and then timeout exception
    /// </summary>
    private async Task UnityWebRequestAsync(UnityWebRequest request, CancellationToken cancellationToken, Action<float> progress, int maxTimesWait) {
        int countStoppedSteps = 0;
        float prevProgressState = request.downloadProgress;

        request.SendWebRequest();

        await Task.Delay(REQUEST_CHECK_COOLDOWN);

        //awaiting
        while (!request.isDone) {
            progress?.Invoke(request.downloadProgress);
            //check cancel
            if (cancellationToken.IsCancellationRequested) {
                request.Abort();
                cancellationToken.ThrowIfCancellationRequested();
                //check timeout
            } else if (IsServerWaitingTimeout(ref countStoppedSteps, ref prevProgressState, request, maxTimesWait)) {
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

        progress?.Invoke(request.downloadProgress);
    }

    /// <summary>
    /// check server timeout
    /// </summary>
    private bool IsServerWaitingTimeout(ref int countStoppedSteps, ref float prevProgressState, UnityWebRequest request, int maxTimesWait) {
        if (prevProgressState == request.downloadProgress) {
            countStoppedSteps++;
        } else {
            prevProgressState = request.downloadProgress;
            countStoppedSteps = 0;
        }

        return countStoppedSteps >= maxTimesWait;
    }
    #endregion

}