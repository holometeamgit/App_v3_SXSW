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

        if (!string.IsNullOrWhiteSpace(Data.preview_teaser_s3_key)) {
            webRequestHandler.GetTextureRequest(Data.preview_teaser_s3_url,
                                         FetchTeaserTextureCallBack,
                                         ErrorFetchTeaserTextureCallBack);
        } else {
            FetchTexture();
        }
        Data.OnDataUpdated += () => { OnDataUpdated?.Invoke(); };
    }

    private void FetchTeaserTextureCallBack(long code, string body, Texture texture) {
        teaserTexture = texture;

        FetchTexture();
    }
    private void ErrorFetchTeaserTextureCallBack(long code, string body) {
        FetchTexture();
    }

    private void FetchTexture() {
        if (!string.IsNullOrWhiteSpace(Data.preview_s3_key)) {
            webRequestHandler.GetTextureRequest(Data.preview_s3_url,
                             FetchTextureCallBack,
                             ErrorFetchTextureCallBack);
        } else {
            OnErrorTextureLoaded?.Invoke();
        }
    }

    private void FetchTextureCallBack(long code, string body, Texture texture) {
        this.texture = texture;
        OnTextureLoaded?.Invoke();
    }
    private void ErrorFetchTextureCallBack(long code, string body) {
        OnErrorTextureLoaded?.Invoke();
    }
}