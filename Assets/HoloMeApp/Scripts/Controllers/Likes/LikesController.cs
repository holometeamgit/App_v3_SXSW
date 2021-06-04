using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Conteroller for sending webrequest about like and unlike stream
/// </summary>
public class LikesController {
    private WebRequestHandler _webRequestHandler;
    private VideoUploader _videoUploaderAPI;


    public LikesController(WebRequestHandler webRequestHandler, VideoUploader videoUploaderAPI) {
        _webRequestHandler = webRequestHandler;
        _videoUploaderAPI = videoUploaderAPI;
    }

    /// <summary>
    /// call webrequest post like for stream 
    /// </summary>
    public void SendRequestLike(long streamId) {
        _webRequestHandler.Post(GetPostLikeRequestUrl(streamId), string.Empty, WebRequestHandler.BodyType.None,
                (code, body) => { HelperFunctions.DevLog("Setted like " + streamId); },
                (code, body) => { HelperFunctions.DevLogError("error Set like " + streamId + " code " + code + " " + body); });
    }

    /// <summary>
    /// call webrequest delete like for stream 
    /// </summary>
    public void SendRequestUnlike(long streamId) {
        _webRequestHandler.Delete(GetDeleteLikeRequestUrl(streamId),
        (code, body) => { HelperFunctions.DevLog("Deleted like " + streamId); },
        (code, body) => { HelperFunctions.DevLogError("error Delete like " + streamId + " code " + code + " " + body); });
    }

    private string GetPostLikeRequestUrl(long streamId) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderAPI.PostLike.Replace("{id}", streamId.ToString());
    }

    private string GetDeleteLikeRequestUrl(long streamId) {
        return _webRequestHandler.ServerURLMediaAPI + _videoUploaderAPI.DeleteLike.Replace("{id}", streamId.ToString());
    }

}
