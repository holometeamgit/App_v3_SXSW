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

    [SerializeField] ServerURLAPIScriptableObject serverURLAPI;

    private const int COUNT_REPEAT = 3;
    private const float TIME_REPEAT = 0.1f;
    private const int TIMEOUT_REQUEST = 5;

    //new
    private const int REQUEST_CHECK_COOLDOWN = 250;
    private const int MAX_COUNT_STOPPED_STEPS_REQUEST = 20;

    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, Action onCancel = null, Action<float> progress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            return PrepareGetRequest(currentUrl, currentHeaderAccessToken);
        };

        //Debug.Log("GetRequest url: " + url);

        //TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        //UnityWebRequest unityWebRequest = PrepareGetRequest(url, headerAccessToken);
        //WebRequestWithRetryAsync(unityWebRequest, onCancel, progress).ContinueWith((taskWebRequestData) => {
        //    Debug.Log("Request recieved data: " + taskWebRequestData.Result.Data);
        //    Debug.Log("Request recieved data: " + unityWebRequest.downloadHandler.text);

        //    //responseDelegate?.Invoke(taskWebRequestData.Result.Code, taskWebRequestData.Result.Data);
        //}, taskScheduler);

        MyRequest(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, progress);
        /*TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        UnityWebRequest unityWebRequest = PrepareGetRequest(url, headerAccessToken);
        WebRequestWithRetryAsync(unityWebRequest, null, null).ContinueWith((task) => {

            Debug.Log("Done!!!!!!: " + unityWebRequest.downloadHandler.text);
            Debug.Log("Task Done!!!!!!: " + task.Result.Data);
        },
            cancellationTokenSource.Token, TaskContinuationOptions.None, taskScheduler);*/

        //StartCoroutine(GetRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
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

        return request;
    }

    /// <summary>
    /// request to server and send call back
    /// </summary>
    private void MyRequest(Func<UnityWebRequest> createWebRequest,
        ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            Action onCancel = null, Action<float> progress = null) {

        UnityWebRequest request = null;
        try {
            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Debug.Log("createWebRequest");

            request = createWebRequest?.Invoke();

            Debug.Log("request: " + request.uri);

            WebRequestWithRetryAsync(request, onCancel, progress).ContinueWith((taskWebRequestData) => {
                Debug.Log("Request recieved data: " + taskWebRequestData.Result.Data);
                Debug.Log("Request recieved data: " + request.downloadHandler.text);

                responseDelegate?.Invoke(taskWebRequestData.Result.Code, taskWebRequestData.Result.Data);
            }, taskScheduler);

        } catch(UnityWebRequestException uwrException) {
            errorTypeDelegate?.Invoke(uwrException.Code, uwrException.Message);
        } catch (UnityWebRequestServerConnectionException uwrServerConnectionException) {
            errorTypeDelegate?.Invoke(uwrServerConnectionException.Code, uwrServerConnectionException.Message);
        } catch {
            errorTypeDelegate?.Invoke(500, "Failed to connect to server");
        } finally {
            request?.Dispose();
        }
    }

    /// <summary>
    /// Async WebRequest with Retry 
    /// </summary>
    private async Task<UnitySuccessWebRequestData<string>> WebRequestWithRetryAsync(UnityWebRequest request,
        Action onCancel = null, Action<float> progress = null) {

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        if (onCancel != null) {
            onCancel += cancellationTokenSource.Cancel;
        }
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        try {
            //await UnityWebRequestAsync(request, cancellationToken, progress);
            Task requestTask = UnityWebRequestAsync(request, cancellationToken, progress);
            await RetryAsyncHelpe.RetryOnExceptionAsync<UnityWebRequestServerConnectionException>(async () => { await requestTask; });
            long code = request.responseCode;
            string text = request.downloadHandler.text;
            return new UnitySuccessWebRequestData<string>(request.responseCode, text);
        } 
        finally {
            if (onCancel != null) {
                onCancel -= cancellationTokenSource.Cancel;
            }
            cancellationTokenSource.Dispose();
        }
    }

    /// <summary>
    /// Unity Asynchronous request
    /// if nothing happens waiting time = REQUEST_CHECK_COOLDOWN * MAX_COUNT_STOPPED_STEPS_REQUEST and then timeout exception
    /// </summary>
    private async Task UnityWebRequestAsync(UnityWebRequest request, CancellationToken cancellationToken, Action<float> progress) {
        int countStoppedSteps = 0;
        float prevProgressState = request.downloadProgress;

        request.SendWebRequest();

        await Task.Delay(REQUEST_CHECK_COOLDOWN);

        //awaiting
        while (request.downloadProgress != 1) {
            progress?.Invoke(request.downloadProgress);
            //check cancel
            if (cancellationToken.IsCancellationRequested) {
                request.Abort();
                cancellationToken.ThrowIfCancellationRequested();
                //check timeout
            } else if (IsServerWaitingTimeout(ref countStoppedSteps, ref prevProgressState, request)) {
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

    private bool IsServerWaitingTimeout(ref int countStoppedSteps, ref float prevProgressState, UnityWebRequest request) {
        if (prevProgressState == request.downloadProgress) {
            countStoppedSteps++;
        } else {
            prevProgressState = request.downloadProgress;
            countStoppedSteps = 0;
        }

        return countStoppedSteps >= MAX_COUNT_STOPPED_STEPS_REQUEST;
    }

    /*private async Task<string> GetAsync() {

        await Task.Delay(4 * timeDelay);

        return "";
    }

    CancellationTokenSource tokenSource2;
    const int timeDelay = 250;

    private void Start() {
        int a = 0;
        int b = 1;
        try {
            b = a + a;
        } finally {
            Debug.Log("Finally");
        }

        Debug.Log("After catch");

        return;
        Action onCnacel = delegate { };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        onCnacel += tokenSource2.Cancel;

        Debug.Log("Start");

        ct.Register(() => {
            Debug.Log("Task is canceled");
        });

        //Task.Delay(4 * timeDelay).ContinueWith((_) => { Debug.Log(4 * timeDelay); });
        //Task.Delay(8 * timeDelay).ContinueWith((_) => { Debug.Log(8 * timeDelay); });
        //Task.Delay(12 * timeDelay).ContinueWith((_) => { Debug.Log(12 * timeDelay); });
        //Task.Delay(16 * timeDelay).ContinueWith((_) => { Debug.Log(16 * timeDelay); });

        onCnacel -= tokenSource2.Cancel;
        Task.Delay(6 * timeDelay).ContinueWith((_) => { onCnacel?.Invoke(); });

        FetchUsersAsync(ct).ContinueWith(task => {
            Debug.Log("Task IsCompleted: " + task.IsCompleted);
            Debug.Log("Task IsCanceled: " + task.IsCanceled);
            Debug.Log("Task IsFaulted:  " + task.IsFaulted);

            if (!task.IsCanceled && !task.IsFaulted) {
                Debug.Log("Completed msg ");
                Debug.Log("Completed msg " + task.Result);
            } else if (task.IsCanceled) {
                Debug.Log("Cancel msg ");
            } else if (task.IsFaulted) {
                Debug.Log("Exception msg ");

                if (task.Exception is AggregateException) // is it an AggregateException?
    {
                    var ae = task.Exception as AggregateException;
                    if (ae.InnerExceptions.Count > 0) {
                        Debug.Log(ae.InnerExceptions[0].Message);
                        WebExceptionJsonData webException = JsonUtility.FromJson<WebExceptionJsonData>(ae.InnerExceptions[0].Message);
                        WebExceptionJsonData webException1 = new WebExceptionJsonData(502, "Timeout");
                        Debug.Log("Example: " + JsonUtility.ToJson(webException));
                        Debug.Log("Ready read error is null " + webException == null);
                        if (webException != null) {
                            Debug.Log(webException.Msg);
                        } else {
                            Debug.Log("can't read msg. It will be Connection Exception");
                        }
                    } else {
                        Debug.Log("Connection Exception");
                    }
                    //foreach (var e in ae.InnerExceptions) // loop them and print their messages
                    //{
                    //    Debug.Log(e.Message); // output is "y" .. because that's what you threw
                    //}
                }
            }
            //tokenSource2.Dispose();
        }
        , taskScheduler);
        //tokenSource2.CancelAfter(6 * timeDelay);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            tokenSource2.Cancel();
        }
    }

    async Task<string> FetchUsersAsync(CancellationToken ct) {
        int i = 0;
        int v = 1;
        int c = 0;
         try {

        await Task.Delay(4 * timeDelay);
        //WebException webException = new WebException(502, "Exception: Timeout");
        //throw new Exception(JsonUtility.ToJson(webException));
        //c = v / i;
        ct.ThrowIfCancellationRequested();
        await Task.Delay(4 * timeDelay);
        ct.ThrowIfCancellationRequested();
        await Task.Delay(4 * timeDelay);
        ct.ThrowIfCancellationRequested();

          } catch (AggregateException ae) {
               return "AggregateException" + ae.Message;
           } catch (Exception e) {
               return "Exception " + e.Message;
           } finally {
               //tokenSource2.Dispose(); // ?????? how that can know about tokenSource2
           }

        return "Fetched";
    }

    async Task<string> Starting(CancellationToken ct) {
        try {
            var users = await FetchUsersAsync(ct);
            return users;
        } catch {
            Debug.Log("An error occurred");
            return "Error";
        }
    }
    */

    #region old
    public void DeleteRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(DeleteRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PostRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null, int countRepeat = COUNT_REPEAT) {
        StartCoroutine(PostRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken, countRepeat));
    }

    public void PostRequestMultipart(string url, byte[] body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PostRequestingMultiPart(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PutRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PutRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PatchRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PatchRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    IEnumerator GetRequesting(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            request.certificateHandler = new CustomCertificateHandler();

            if (headerAccessToken != null) {
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
            }

            request.timeout = TIMEOUT_REQUEST;

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
        }
    }

    //IEnumerator PostRequestingMultiPart(string url, byte[] myData, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
    //    {

    //        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, myData)) {
    //            webRequest.method = "POST";
    //            webRequest.SetRequestHeader("Content-Type", "multipart/form-data");
    //            if (headerAccessToken != null)
    //                webRequest.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

    //            yield return webRequest.Send();

    //            if (webRequest.isNetworkError || webRequest.isHttpError) {
    //                HelperFunctions.DevLog("ErrorCode :" + webRequest.responseCode + ": Error: " + webRequest.error + webRequest.downloadHandler.text);
    //                errorTypeDelegate(webRequest.responseCode, webRequest.downloadHandler.text);
    //            } else {
    //                Debug.Log("Upload complete!");
    //                responseDelegate(webRequest.responseCode, webRequest.downloadHandler.text);
    //            }
    //        }
    //    }
    //}

    IEnumerator PostRequestingMultiPart(string url, byte[] myData, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
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
        UnityWebRequest wr = new UnityWebRequest(url, "POST");
        UploadHandler uploader = new UploadHandlerRaw(body);
        uploader.contentType = contentType;
        wr.uploadHandler = uploader;

        //wr.SetRequestHeader("Content-Type", "multipart/form-data");
        //wr.SetRequestHeader("Content-Type", "application/json");

        if (headerAccessToken != null)
            wr.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
        wr.timeout = TIMEOUT_REQUEST + TIMEOUT_REQUEST;
        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError) {
            //            Debug.Log(request.responseCode + " : " + request.error);
            errorTypeDelegate(wr.responseCode, wr.error);
        } else {
            //            Debug.Log(request.responseCode + " : " + request.downloadHandler.text);
            responseDelegate(wr.responseCode, wr.error);
        }
    }

    #endregion

    IEnumerator PostRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null, int countRepeat = 0) {
        byte[] bodyRaw = new byte[0];

        UnityWebRequest request;// = new UnityWebRequest(url, "POST");

        try {
            switch (bodyType) {
                case BodyType.XWWWFormUrlEncoded:
                    Debug.Log("Post XWWWFormUrlEncoded");
                    WWWForm form = new WWWForm();
                    Type listType = typeof(T);
                    if (listType == typeof(Dictionary<string, string>)) {
                        Dictionary<string, string> fields = body as Dictionary<string, string>;
                        foreach (var field in fields) {
                            form.AddField(field.Key, field.Value);
                            Debug.Log("add field " + field.Key + " " + field.Value);
                        }
                    } else if (listType == typeof(Dictionary<string, byte[]>)) {
                        Dictionary<string, byte[]> fields = body as Dictionary<string, byte[]>;
                        foreach (var field in fields) {
                            form.AddBinaryData(field.Key, field.Value);
                            Debug.Log("add field " + field.Key + " " + field.Value);
                        }
                    }
                    request = UnityWebRequest.Post(url, form);
                    request.timeout = 5;
                    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    break;
                case BodyType.JSON: //only Json at this moment

                    string bodyString = JsonUtility.ToJson(body);
                    bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                    request = new UnityWebRequest(url, "POST");
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.uploadHandler = new UploadHandlerRaw(data: bodyRaw);
                    break;
                case BodyType.None:
                default:
                    request = new UnityWebRequest(url, "POST");
                    break;
            }

            //  if(bodyType != BodyType.None && body != null)

            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            if (headerAccessToken != null)
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        } catch (System.Exception e) {
            Debug.Log("post error web request " + e);
            errorTypeDelegate?.Invoke(500, "Sorry, problems with the request to the server");
            yield break;
        }

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            HelperFunctions.DevLogError(request.responseCode + " : " + request.error);
            if (request.responseCode == 500 && countRepeat > 0) {
                HelperFunctions.DevLogError("Server 500");
                countRepeat--;
                yield return new WaitForSeconds(TIME_REPEAT * (COUNT_REPEAT - countRepeat));
                PostRequest(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken, countRepeat);
            } else {
                errorTypeDelegate(request.responseCode, request.downloadHandler.text);
            }
        } else {
            responseDelegate(request.responseCode, request.downloadHandler.text);

        }
    }

    IEnumerator PutRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PUT");
        try {
            switch (bodyType) {
                default: //only Json at this moment
                    string bodyString = JsonUtility.ToJson(body);
                    bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                    break;
            }

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = TIMEOUT_REQUEST;
            if (headerAccessToken != null)
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        } catch (System.Exception e) {
            Debug.Log("post error web request " + e);
            yield break;
        }

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
            responseDelegate(request.responseCode, request.downloadHandler.text);
        }
    }

    IEnumerator PatchRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PATCH");

        try {
            switch (bodyType) {
                default: //only Json at this moment
                    string bodyString = JsonUtility.ToJson(body);
                    bodyRaw = Encoding.UTF8.GetBytes(bodyString);
                    break;
            }

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = TIMEOUT_REQUEST;
            if (headerAccessToken != null)
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
        } catch (System.Exception e) {
            Debug.Log("post error web request " + e);
            yield break;
        }
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
            responseDelegate(request.responseCode, request.downloadHandler.text);
        }
    }

    IEnumerator DeleteRequesting(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {

        Debug.Log(url);

        using (UnityWebRequest request = UnityWebRequest.Delete(url)) {
            request.certificateHandler = new CustomCertificateHandler();
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            if (headerAccessToken != null)
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
            request.timeout = TIMEOUT_REQUEST;
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
            request?.Dispose();
        }

    }

    public void LogCallback(long code, string body) {
        HelperFunctions.DevLog($"Code {code} Message {body}");
    }

    public void ErrorLogCallback(long code, string body) {
        HelperFunctions.DevLogError($"An Error Occurred! Code {code} Message {body}");
    }

}