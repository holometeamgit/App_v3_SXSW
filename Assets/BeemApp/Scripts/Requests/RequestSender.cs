using System;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {
    /// <summary>
    /// Request Process
    /// </summary>
    public class RequestSender : IRequestSender {

        /// <summary>
        /// Send Request
        /// </summary>
        public async void Send(UnityWebRequest webRequest, Action<string> Success = null, Action<string> Fail = null) {

            UnityWebRequest.Result result = await webRequest.SendWebRequest();

            if (result == UnityWebRequest.Result.Success) {
                HelperFunctions.DevLog($"Success: {webRequest.downloadHandler.text}", "Request");
                Success?.Invoke(webRequest.downloadHandler.text);
            } else {
                HelperFunctions.DevLogError($"Failed: {webRequest.error}", "Request");
                Fail?.Invoke(webRequest.error);
            }
        }
    }
}
