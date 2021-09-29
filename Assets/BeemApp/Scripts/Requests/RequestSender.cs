using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {
    public class RequestSender : IRequestSender {

        /// <summary>
        /// Send Request
        /// </summary>
        public async void Send(UnityWebRequest webRequest, Action<string> Success = null, Action<string> Fail = null) {

            UnityWebRequest.Result result = await webRequest.SendWebRequest();

            Debug.LogError("Request Completed");

            if (result == UnityWebRequest.Result.Success) {
                Debug.Log($"Success: {webRequest.downloadHandler.text}");
                Success?.Invoke(webRequest.downloadHandler.text);
            } else {
                Debug.LogError($"Failed: {webRequest.error}");
                Fail?.Invoke(webRequest.error);
            }
        }
    }
}
