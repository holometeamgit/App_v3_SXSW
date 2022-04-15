using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Get StadiumController
/// </summary>
public class GetStadiumController {
    private VideoUploader _videoUploaderScriptableObject;
    private WebRequestHandler _webRequestHandler;

    private const string STATUS = "live";
    private const string USERNAME_FILTER = "user__username";
    private const string STATUS_FILTER = "status";

    /// <summary>
    /// Constructor for GetStadiumController
    /// </summary>
    /// <param name="videoUploaderScriptableObject"></param>
    /// <param name="webRequestHandler"></param>
    public GetStadiumController(VideoUploader videoUploaderScriptableObject, WebRequestHandler webRequestHandler) {
        _videoUploaderScriptableObject = videoUploaderScriptableObject;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Get Stadium By username
    /// </summary>
    /// <param name="id"></param>
    public void GetStadiumByUsername(string username, Action<StreamJsonData.Data> onSuccess = null, Action<WebRequestError> onFailed = null) {
        _webRequestHandler.Get(GetStadiumByUsername(username), (code, body) => { OnSuccess(code, body, username, onSuccess, onFailed); }, (code, body) => { OnFailed(code, body, onFailed); });
    }

    private void OnSuccess(long code, string body, string username, Action<StreamJsonData.Data> onSuccess, Action<WebRequestError> onFailed) {
        StreamJsonData data = JsonUtility.FromJson<StreamJsonData>(body);
        if (data.results.Count > 0) {
            StreamJsonData.Data lastStreamData = null;

            data.results.RemoveAll(x => !(x.GetStage() == StreamJsonData.Data.Stage.Prerecorded && x.HasStreamUrl) && !(x.GetStage() == StreamJsonData.Data.Stage.Live));

            if (data.results.Count > 0) {
                foreach (StreamJsonData.Data item in data.results) {
                    if (lastStreamData != null) {
                        if (item.StartDate.CompareTo(lastStreamData.StartDate) < 0 && item.user == username) {
                            lastStreamData = item;
                        }
                    } else {
                        if (item.user == username) {
                            lastStreamData = item;
                        }
                    }
                }

                if (lastStreamData != null) {
                    if ((lastStreamData.GetStage() == StreamJsonData.Data.Stage.Prerecorded && lastStreamData.HasStreamUrl) || lastStreamData.GetStage() == StreamJsonData.Data.Stage.Live) {
                        onSuccess?.Invoke(lastStreamData);
                    } else {
                        onFailed?.Invoke(new WebRequestError());
                    }
                }
            } else {
                lastStreamData = new StreamJsonData.Data {
                    user = username,
                    status = StreamJsonData.Data.STOP_STR
                };
                onSuccess?.Invoke(lastStreamData);
            }
        } else {
            onFailed?.Invoke(new WebRequestError());
        }
    }

    private void OnFailed(long code, string body, Action<WebRequestError> onFailed) {
        HelperFunctions.DevLogError("Failed" + code + " " + body);
        onFailed?.Invoke(new WebRequestError(code, body));
    }

    private string GetStadiumByUsername(string username) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderScriptableObject.Stream + $"?{USERNAME_FILTER}={username}";
    }
}
