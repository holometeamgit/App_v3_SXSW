using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

public class WebRequestHandler : MonoBehaviour
{
    public enum BodyType {
        JSON
    }


    public readonly string serverURLAuthAPI = "https://devholo.me/api-auth";

    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {
        StartCoroutine(GetRequesting(url, responseDelegate, errorTypeDelegate));
    }

    public void PostRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {
        StartCoroutine(PostRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate));
    }

    IEnumerator GetRequesting(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {

        Debug.Log(url);

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            request.certificateHandler = new CustomCertificateHandler();

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
        }
    }

    IEnumerator PostRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "POST");

        switch (bodyType) {
        default: //only Json at this moment
            string bodyString = JsonUtility.ToJson(body);
            Debug.Log("Post data " + bodyString);
            bodyRaw = Encoding.UTF8.GetBytes(bodyString);
            break;
        }

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("url " + url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.Log(request.responseCode + " : " + request.error);
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
            Debug.Log(request.responseCode + " : " + request.downloadHandler.text);
            responseDelegate(request.responseCode, request.downloadHandler.text);
        }
    }
}
