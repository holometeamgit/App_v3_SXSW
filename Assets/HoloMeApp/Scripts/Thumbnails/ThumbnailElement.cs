using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThumbnailElement {
    public long Id { get; }
    public StreamJsonData.Data Data { get; }
    public Texture texture;
    public Texture teaserTexture;

    public Action OnTextureLoaded;
    public Action OnErrorTextureLoaded;
    public Action OnDataUpdated;

    WebRequestHandler webRequestHandler;

    public ThumbnailElement(StreamJsonData.Data data, WebRequestHandler webRequestHandler) {
        texture = null;
        teaserTexture = null;
        Data = data;
        Id = data.id;
        this.webRequestHandler = webRequestHandler;

        //TODO optimize 
        string teaserTextureUrl = string.IsNullOrWhiteSpace(Data.preview_teaser_s3_url) ? "1" : Data.preview_teaser_s3_url;
        webRequestHandler.GetTextureRequest(teaserTextureUrl,
                                     FetchTeaserTextureCallBack,
                                     ErrorFetchTeaserTextureCallBack);

        Data.OnDataUpdated += () => { OnDataUpdated?.Invoke(); };
    }

    private void FetchTeaserTextureCallBack(long code, string body, Texture texture) {
        teaserTexture = texture;

        FetchTexture();
    }
    private void ErrorFetchTeaserTextureCallBack(long code, string body) {
//        Debug.Log("ErrorFetchTeaserTextureCallBack");
        FetchTexture();
    }

    private void FetchTexture() {
        //TODO optimize 
        string textureUrl = string.IsNullOrWhiteSpace(Data.preview_s3_url) ? "1" : Data.preview_s3_url;
        webRequestHandler.GetTextureRequest(textureUrl,
                             FetchTextureCallBack,
                             ErrorFetchTextureCallBack);
    }

    private void FetchTextureCallBack(long code, string body, Texture texture) {
        HelperFunctions.DevLog("FetchTextureCallBack: " + body);
        this.texture = texture;
        OnTextureLoaded?.Invoke();
    }
    private void ErrorFetchTextureCallBack(long code, string body) {
        HelperFunctions.DevLog("ErrorFetchTextureCallBack: " + body);
        OnErrorTextureLoaded?.Invoke();
    }
}