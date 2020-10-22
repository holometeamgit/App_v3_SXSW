using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThumbnailElement {
    public long Id { get; }
    public StreamJsonData.Data Data { get; }
    public Texture texture;

    public Action<Texture> OnTextureLoaded;
    public Action OnErrorTextureLoaded;

    public ThumbnailElement(StreamJsonData.Data data, MediaFileDataHandler mediaFileDataHandler) {
        Data = data;
        Id = data.id;

        string textureUrl = string.IsNullOrWhiteSpace(Data.preview_s3_url) ? "1" : Data.preview_s3_url;
        mediaFileDataHandler.LoadImg(textureUrl,
                                     FetchTexture,
                                     ErrorFetchTexture);
    }

    private void FetchTexture(long code, string body, Texture texture) {
//        Debug.Log("OnTextureLoaded");
        this.texture = texture;
        OnTextureLoaded?.Invoke(texture);
    }
    private void ErrorFetchTexture(long code, string body) {
//        Debug.Log("OnErrorTextureLoaded");
        OnErrorTextureLoaded?.Invoke();
    }

}