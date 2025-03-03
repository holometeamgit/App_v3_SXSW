﻿using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AgoraRequests : MonoBehaviour {
    const string baseURL = "https://api.agora.io"; // "https://api.agora.io/dev";
    //const string baseURL = "https://devholo.me/api-media";
    const string customerID = "8bf4ed6d09574f1580767818f6c73200";
    const string certificate = "bd8dfbe35d0e443cb43f2ed51b2fa3d3"; //Secret Key

    //public enum RequestType {PUT , POST, PATCH, GET };

    public static string Base64Encode(string plainText) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    IEnumerator GetRequest(string uri, Action<string> OnSuccess, Action OnFailed) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            string authorizationDetails = customerID + ":" + certificate;
            string encodedAuthDetails = Base64Encode(authorizationDetails);
            webRequest.SetRequestHeader("Authorization", "Basic " + encodedAuthDetails);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            HandleReturn(OnSuccess, OnFailed, webRequest, pages, page);
        }
    }

    IEnumerator PostRequest(string uri, string body, Action<string> OnSuccess, Action OnFailed) {

        var webRequest = new UnityWebRequest(uri, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

        webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        string authorizationDetails = customerID + ":" + certificate;
        string encodedAuthDetails = Base64Encode(authorizationDetails);

        webRequest.SetRequestHeader("Authorization", "Basic " + encodedAuthDetails);
        webRequest.SetRequestHeader("Content-type", "application/json;charset=utf-8");

        yield return webRequest.SendWebRequest();

        string[] pages = uri.Split('/');
        int page = pages.Length - 1;

        HandleReturn(OnSuccess, OnFailed, webRequest, pages, page);


        //using (UnityWebRequest webRequest = UnityWebRequest.Put(url, Encoding.UTF8.GetBytes(body))) {
        //    string authorizationDetails = customerID + ":" + certificate;
        //    string encodedAuthDetails = Base64Encode(authorizationDetails);

        //    webRequest.SetRequestHeader("Authorization", "Basic " + encodedAuthDetails);
        //    webRequest.SetRequestHeader("Content-type", "application/json;charset=utf-8");

        //    // Request and wait for the desired page.
        //    yield return webRequest.SendWebRequest();

        //    string[] pages = url.Split('/');
        //    int page = pages.Length - 1;

        //    if (webRequest.isNetworkError) {
        //        HelperFunctions.DevLog(pages[page] + ": Error: " + webRequest.error);
        //        OnFailed?.Invoke();
        //    } else {
        //        HelperFunctions.DevLog(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text + webRequest.responseCode);
        //        OnSuccess?.Invoke(webRequest.downloadHandler.text);
        //    }
        //}
    }

    private static void HandleReturn(Action<string> OnSuccess, Action OnFailed, UnityWebRequest webRequest, string[] pages, int page) {
        if (webRequest.isNetworkError || webRequest.isHttpError) {
            HelperFunctions.DevLog(pages[page] + ": ErrorCode :" + webRequest.responseCode + ": Error: " + webRequest.error + webRequest.downloadHandler.text);
            OnFailed?.Invoke();
        } else {
            HelperFunctions.DevLog(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text + webRequest.responseCode);
            OnSuccess(webRequest.downloadHandler.text);
        }
    }

    public void MakeGetRequest(RestRequest restRequest) {
        HelperFunctions.DevLog(baseURL + restRequest.RequestString);
        StartCoroutine(GetRequest(baseURL + restRequest.RequestString /*+ appId*/, restRequest.OnSuccess, restRequest.OnFailed));
    }

    public void MakePostRequest(RestRequest restRequest, string data) {
        HelperFunctions.DevLog(baseURL + restRequest.RequestString);
        StartCoroutine(PostRequest(baseURL + restRequest.RequestString, data, restRequest.OnSuccess, restRequest.OnFailed));
    }

    //void RequestChannels()
    //{
    //    StartCoroutine(GetRequest(baseURL + "v1/channel/" + appId, OnChannelsReturned));
    //}

    //void OnChannelsReturned(string jsonText)
    //{
    //    var response = JsonParser.CreateFromJSON<ChannelResponse>(jsonText);
    //}

}
