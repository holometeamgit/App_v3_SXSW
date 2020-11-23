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

    MediaFileDataHandler mediaFileDataHandler;

    public ThumbnailElement(StreamJsonData.Data data, MediaFileDataHandler mediaFileDataHandler) {
        texture = null;
        teaserTexture = null;
        Data = data;
        Id = data.id;
        this.mediaFileDataHandler = mediaFileDataHandler;

        //TODO optimize 
        string teaserTextureUrl = string.IsNullOrWhiteSpace(Data.preview_teaser_s3_url) ? "1" : Data.preview_teaser_s3_url;
        mediaFileDataHandler.LoadImg(teaserTextureUrl,
                                     FetchTeaserTextureCallBack,
                                     ErrorFetchTeaserTextureCallBack);
    }

    private void FetchTeaserTextureCallBack(long code, string body, Texture texture) {
        teaserTexture = texture;

        FetchTexture();
    }
    private void ErrorFetchTeaserTextureCallBack(long code, string body) {
        FetchTexture();
    }

    private void FetchTexture() {
        //TODO optimize 
        string textureUrl = string.IsNullOrWhiteSpace(Data.preview_s3_url) ? "1" : Data.preview_s3_url;
        mediaFileDataHandler.LoadImg(textureUrl,
                             FetchTextureCallBack,
                             ErrorFetchTextureCallBack);
    }

    private void FetchTextureCallBack(long code, string body, Texture texture) {
        this.texture = texture;
        OnTextureLoaded?.Invoke();
    }
    private void ErrorFetchTextureCallBack(long code, string body) {
        OnErrorTextureLoaded?.Invoke();
    }
}