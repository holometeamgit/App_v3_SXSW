using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

public class WebRequestHandler : MonoBehaviour {
    public enum BodyType {
        JSON
    }

    public string ServerURLAuthAPI { get { return serverURLAPI.ServerURLAuthAPI; } private set { } }
    public string ServerURLMediaAPI { get { return serverURLAPI.ServerURLMediaAPI; } private set { } }

    [SerializeField] ServerURLAPIScriptableObject serverURLAPI;

    public void GetRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(GetRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void DeleteRequest(string url, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(DeleteRequesting(url, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PostRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PostRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PostRequestMultipart(string url, byte[] body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PostRequestingMultiPart(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PutRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PutRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
    }

    public void PatchRequest<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        StartCoroutine(PatchRequesting(url, body, bodyType, responseDelegate, errorTypeDelegate, headerAccessToken));
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

    //IEnumerator PostRequestingMultiPart(string url, byte[] myData, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
    //    {

    //        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, myData)) {
    //            webRequest.method = "POST";
    //            webRequest.SetRequestHeader("Content-Type", "multipart/form-data");
    //            if (headerAccessToken != null)
    //                webRequest.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

    //            yield return webRequest.Send();

    //            if (webRequest.isNetworkError || webRequest.isHttpError) {
    //                HelperFunctions.DevLog("ErrorCode :" + webRequest.responseCode + ": Error: " + webRequest.error + webRequest.downloadHandler.text);
    //                errorTypeDelegate(webRequest.responseCode, webRequest.downloadHandler.text);
    //            } else {
    //                Debug.Log("Upload complete!");
    //                responseDelegate(webRequest.responseCode, webRequest.downloadHandler.text);
    //            }
    //        }
    //    }
    //}

    IEnumerator PostRequestingMultiPart(string url, byte[] myData, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        // read a file and add it to the form
        List<IMultipartFormSection> form = new List<IMultipartFormSection> { new MultipartFormFileSection("image", myData, "VideoThumbnail.png", "image") };
        // generate a boundary then convert the form to byte[]
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);
        // my termination string consisting of CRLF--{boundary}--
        byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
        // Make my complete body from the two byte arrays
        byte[] body = new byte[formSections.Length + terminate.Length];
        Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
        Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);
        // Set the content type - NO QUOTES around the boundary
        string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));
        // Make my request object and add the raw body. Set anything else you need here
        UnityWebRequest wr = new UnityWebRequest(url, "POST");
        UploadHandler uploader = new UploadHandlerRaw(body);
        uploader.contentType = contentType;
        wr.uploadHandler = uploader;

        //wr.SetRequestHeader("Content-Type", "multipart/form-data");
        //wr.SetRequestHeader("Content-Type", "application/json");

        if (headerAccessToken != null)
            wr.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

        yield return wr.SendWebRequest();

        if (wr.isNetworkError || wr.isHttpError) {
            //            Debug.Log(request.responseCode + " : " + request.error);
            errorTypeDelegate(wr.responseCode, wr.error);
        } else {
            //            Debug.Log(request.responseCode + " : " + request.downloadHandler.text);
            responseDelegate(wr.responseCode, wr.error);
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

    IEnumerator PatchRequesting<T>(string url, T body, BodyType bodyType, ResponseDelegate responseDelegate, ErrorTypeDelegate errorTypeDelegate, string headerAccessToken = null) {
        byte[] bodyRaw;
        var request = new UnityWebRequest(url, "PATCH");

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
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            if (headerAccessToken != null)
                request.SetRequestHeader("Authorization", "Bearer " + headerAccessToken);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError) {
                errorTypeDelegate(request.responseCode, request.error);
            } else {
                responseDelegate(request.responseCode, request.downloadHandler.text);
            }
        }
    }

    public void LogCallback(long code, string body) {
        HelperFunctions.DevLog($"Code {code} Message {body}");
    }

    public void ErrorLogCallback(long code, string body) {
        HelperFunctions.DevLogError($"An Error Occurred! Code {code} Message {body}");
    }

}
