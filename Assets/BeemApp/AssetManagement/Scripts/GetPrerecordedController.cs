using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get PrerecordedController
/// </summary>
public class GetPrerecordedController {
    private VideoUploader _videoUploaderScriptableObject;
    private WebRequestHandler _webRequestHandler;

    /// <summary>
    /// Constructor for GetPrerecordedController
    /// </summary>
    /// <param name="videoUploaderScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetPrerecordedController(VideoUploader videoUploaderScriptableObject, WebRequestHandler webRequestHandler) {
        _videoUploaderScriptableObject = videoUploaderScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get Prerecorded By Slug
    /// </summary>
    /// <param name="id"></param>
    public void GetPrerecordedBySlug(string slug, Action<StreamJsonData.Data> onSuccess = null, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetPrerecordedBySlug(slug), (code, body) => { OnSuccess(code, body, onSuccess, onFailed); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, Action<StreamJsonData.Data> onSuccess, Action<WebRequestError> onFailed) {
        StreamJsonData.Data data = JsonUtility.FromJson<StreamJsonData.Data>(body);
        if (data != null) {
            if ((data.GetStage() == StreamJsonData.Data.Stage.Prerecorded && data.HasStreamUrl) || data.GetStage() == StreamJsonData.Data.Stage.Live) {
                onSuccess?.Invoke(data);
            } else {
                onFailed?.Invoke(new WebRequestError());
            }
        } else {
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetPrerecordedBySlug(string slug) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderScriptableObject.StreamBySlug.Replace("{slug}", slug);
    }
}
