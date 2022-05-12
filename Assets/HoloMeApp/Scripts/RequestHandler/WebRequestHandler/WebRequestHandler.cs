using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Zenject;
/// <summary>
/// WebRequestHandler containt all webrequest function
/// </summary>
public class WebRequestHandler : MonoBehaviour {

    public string ServerURLAuthAPI { get { return serverURLAPI.ServerURLAuthAPI; } private set { } }
    public string ServerURLMediaAPI { get { return serverURLAPI.ServerURLMediaAPI; } private set { } }
    public string ServerProvidersAPI { get { return serverURLAPI.ServerProvidersAPI; } private set { } }

    [SerializeField] ServerURLAPIScriptableObject serverURLAPI;

    private bool _isInit;
    private GetWebRequester _getWebRequester;
    private GetWebTextureRequest _getWebTextureRequest;
    private PostWebRequester _postWebRequester;
    private PostMultipartRequester _postMultipartRequester;
    private PatchMultipartRequester _patchMultipartRequester;
    private PutWebRequester _putWebRequester;
    private PatchWebRequester _patchWebRequester;
    private DeleteWebRequester _deleteWebRequester;
    private AccountManager _accountManager;

    [Inject]
    public void Construct(AccountManager accountManager) {
        _accountManager = accountManager;
    }


    private void Awake() {
        Construct();
    }

    private void Construct() {
        Init();
    }

    private void Init() {
        if (_isInit)
            return;
        _getWebRequester = new GetWebRequester();
        _getWebTextureRequest = new GetWebTextureRequest();
        _postWebRequester = new PostWebRequester();
        _postMultipartRequester = new PostMultipartRequester();
        _patchMultipartRequester = new PatchMultipartRequester();
        _putWebRequester = new PutWebRequester();
        _patchWebRequester = new PatchWebRequester();
        _deleteWebRequester = new DeleteWebRequester();

        _isInit = true;
    }

    /// <summary>
    /// default success log Callback
    /// </summary>
    public void LogCallback(long code, string body) {
        HelperFunctions.DevLog($"Code {code} Message {body}");
    }

    /// <summary>
    /// default fail log Callback
    /// </summary>
    public void ErrorLogCallback(long code, string body) {
        HelperFunctions.DevLogError($"An Error Occurred! Code {code} Message {body}");
    }

    /// <summary>
    /// get webrequest
    /// </summary>
    public void Get(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> progress = null) {
        Init();

        _getWebRequester.GetRequest(url, responseDelegate, errorTypeDelegate,
            needHeaderAccessToken ? _accountManager.GetAccessToken().access : null,
            onCancel, progress);
    }

    /// <summary>
    /// post webrequest
    /// </summary>
    public void Post<T>(string url, T body, WebRequestBodyType bodyType,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> progress = null) {
        Init();

        _postWebRequester.PostRequest(url, body, bodyType, responseDelegate, errorTypeDelegate,
            needHeaderAccessToken ? _accountManager.GetAccessToken().access : null,
            onCancel, progress);
    }

    /// <summary>
    /// Delete webrequest
    /// </summary>
    public void Delete(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _deleteWebRequester.DeleteRequest(url, responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel);
    }

    /// <summary>
    /// Delete webrequest
    /// </summary>
    public void Put<T>(string url, T body, WebRequestBodyType bodyType,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> progress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _putWebRequester.PutRequest(url, body, bodyType, responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel, progress);
    }

    /// <summary>
    /// Post request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and path to file</param>
    public void PostMultipart(string url, Dictionary<string, string> contentPathDataDictionary,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _postMultipartRequester.PostMultipart(url, contentPathDataDictionary,
            responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel: onCancel, uploadProgress: uploadProgress);
    }

    /// <summary>
    /// Post request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and binary file</param>
    public void PostMultipart(string url, Dictionary<string, MultipartRequestBinaryData> contentDictionary,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _postMultipartRequester.PostMultipart(url, contentDictionary,
            responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel: onCancel, uploadProgress: uploadProgress);

    }

    /// <summary>
    /// Patch request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and path to file</param>
    public void PatchMultipart(string url, Dictionary<string, string> contentPathDataDictionary,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _patchMultipartRequester.PatchMultipart(url, contentPathDataDictionary,
            responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel: onCancel, uploadProgress: uploadProgress);
    }

    /// <summary>
    /// Patch request multiple files 
    /// </summary>
    /// <param name="contentDictionary"> contain field name and binary file</param>
    public void PatchMultipart(string url, Dictionary<string, MultipartRequestBinaryData> contentDictionary,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> uploadProgress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _patchMultipartRequester.PatchMultipart(url, contentDictionary,
            responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel: onCancel, uploadProgress: uploadProgress);

    }

    /// <summary>
    /// Patch webrequest
    /// </summary>
    public void PatchRequest<T>(string url, T body, WebRequestBodyType bodyType,
            ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
            bool needHeaderAccessToken = true, ActionWrapper onCancel = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _patchWebRequester.PatchRequest(url, body, bodyType,
            responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel);

    }


    /// <summary>
    /// get texture webrequest
    /// </summary>
    public void GetTextureRequest(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate,
        bool needHeaderAccessToken = true, ActionWrapper onCancel = null, Action<float> progress = null) {
        Init();

        string currentHeaderAccessToken = needHeaderAccessToken ? _accountManager.GetAccessToken().access : null;
        _getWebTextureRequest.GetTextureRequest(url, responseDelegate, errorTypeDelegate,
            currentHeaderAccessToken,
            onCancel, progress);
    }
}