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
    public readonly string serverURLMediaAPI = "https://devholo.me/api-media";

    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(GetRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void DeleteRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(DeleteRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PostRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PostRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PutRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PutRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    IEnumerator GetRequesting(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {

        Debug.Log(url);

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            request.certificateHandler = new CustomCertificateHandler();

            if (headerAccessToken != null) {
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
                Debug.Log(request.GetRequestHeader("Authorization"));
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
        }
    }

    IEnumerator PostRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "POST");

        switch (bodyType) {
        default: //only Json at this moment
            string bodyString = JsonUtility.ToJson(body);
            bodyRaw = Encoding.UTF8.GetBytes(bodyString);
            break;
        }

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
//            Debug.Log(request.responseCode + " : " + request.error);
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
//            Debug.Log(request.responseCode + " : " + request.downloadHandler.text);
            responseDelegate(request.responseCode, request.downloadHandler.text);
        }
    }

    IEnumerator PutRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PUT");

        switch (bodyType) {
        default: //only Json at this moment
            string bodyString = JsonUtility.ToJson(body);
            bodyRaw = Encoding.UTF8.GetBytes(bodyString);
            break;
        }

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (headerAccessToken != null)
            request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            errorTypeDelegate(request.responseCode, request.downloadHandler.text);
        } else {
            responseDelegate(request.responseCode, request.downloadHandler.text);
        }
    }

    IEnumerator DeleteRequesting(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {

        Debug.Log(url);

        using (UnityWebRequest request = UnityWebRequest.Delete(url)) {
            request.certificateHandler = new CustomCertificateHandler();

            if (headerAccessToken != null) {
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);
                Debug.Log(request.GetRequestHeader("Authorization"));
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
        }
    }

}
