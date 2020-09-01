using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MediaFileDataHandler : MonoBehaviour
{
    public void LoadImg(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {
        StartCoroutine(GetRequestTexture(url, responseDelegate, errorTypeDelegate));
    }

    IEnumerator GetRequestTexture(string url, ResponseTextureDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            responseDelegate.Invoke(request.responseCode, request.downloadHandler.text, texture);
        }
    }
}
