using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
/// <summary>
/// PostMultipartRequester for Post Multipart Web Request
/// </summary>
public class PostMultipartRequester : WebRequester {

    private const int TIMEOUT_MULTIPART_REQUEST = 1200000;
    private const int MAX_TIMES_BEFORE_POST_MULTIPART_STOP_REQUEST = 150;
    /// <summary>
    /// Post request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and path to file</param>
    public void PostMultipart(string url, Dictionary<string, string> contentPathDataDictionary, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
    string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {
        Dictionary<string, MultipartRequestBinaryData> contentDictionary = new Dictionary<string, MultipartRequestBinaryData>();
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        contentDictionary = LoadingFile(contentPathDataDictionary);

        PostMultipart(url, contentDictionary, responseDelegate, errorTypeDelegate, headerAccessToken, onCancel: onCancel, uploadProgress: uploadProgress);


    }

    /// <summary>
    /// Post request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and binary file</param>
    public void PostMultipart(string url, Dictionary<string, MultipartRequestBinaryData> contentDictionary, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        string headerAccessToken = null, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {

        Func<UnityWebRequest> createWebRequest = () => {
            string currentUrl = url;
            string currentHeaderAccessToken = headerAccessToken;
            Dictionary<string, MultipartRequestBinaryData> currectData = contentDictionary;
            return PreparePostMultipartRequest(currentUrl, currectData, currentHeaderAccessToken);
        };

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WebRequestWithRetryAsync(createWebRequest, responseDelegate, errorTypeDelegate, onCancel, uploadProgress: uploadProgress, maxTimesWait: MAX_TIMES_BEFORE_POST_MULTIPART_STOP_REQUEST).ContinueWith((taskWebRequestData) => {
        }, taskScheduler);
    }

    /// <summary>
    /// Prepare UnityWebRequest for post Multipart request text data
    /// </summary>
    private UnityWebRequest PreparePostMultipartRequest(string url, Dictionary<string, MultipartRequestBinaryData> contentDictionary, string headerAccessToken = null) {
        WWWForm form = new WWWForm();

        foreach (var content in contentDictionary) {
            form.AddBinaryData(content.Key, content.Value.Content, content.Value.FileName);
        }
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.certificateHandler = new CustomCertificateHandler();
        request.downloadHandler = new DownloadHandlerBuffer();

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        request.timeout = TIMEOUT_MULTIPART_REQUEST;

        return request;
    }

    #region async local fileLoading
    private Dictionary<string, MultipartRequestBinaryData> LoadingFile(Dictionary<string, string> filePathsByName) {
        Dictionary<string, MultipartRequestBinaryData> data = new Dictionary<string, MultipartRequestBinaryData>();

        foreach (var path in filePathsByName) {

            byte[] result = File.ReadAllBytes(path.Value);
            data.Add(path.Key, new MultipartRequestBinaryData(path.Key, result, Path.GetFileName(path.Value)));

        }

        return data;
    }
    #endregion

}
