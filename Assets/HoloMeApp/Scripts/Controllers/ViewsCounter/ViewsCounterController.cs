using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Conteroller for sending webrequest that stream was viewed 
/// </summary>
public class ViewsCounterController
{
    private WebRequestHandler _webRequestHandler;
    private VideoUploader _videoUploaderAPI;


    public ViewsCounterController(WebRequestHandler webRequestHandler, VideoUploader videoUploaderAPI) {
        _webRequestHandler = webRequestHandler;
        _videoUploaderAPI = videoUploaderAPI;
    }

    /// <summary>
    /// call webrequest post like for stream 
    /// </summary>
    public void SendViewed(long streamId) {
        _webRequestHandler.Post(GetPostViewRequestUrl(streamId), string.Empty, WebRequestHandler.BodyType.None,
                (code, body) => { HelperFunctions.DevLog("Setted like " + streamId); },
                (code, body) => { HelperFunctions.DevLogError("error Set like " + streamId + " code " + code + " " + body); });
    }

    private string GetPostViewRequestUrl(long streamId) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderAPI.PostView.Replace("{id}", streamId.ToString());
    }
}
