using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {

    /// <summary>
    /// Get Request
    /// </summary>
    public class GetRequest : IRequest {

        private string _url;
        private string _headerAccessToken;

        public GetRequest(string url, string headerAccessToken = null) {
            _url = url;
            _headerAccessToken = headerAccessToken;
        }

        /// <summary>
        /// Send Request
        /// </summary>

        public async void Send(Action<string> Success = null, Action<string> Fail = null, Action<float> Progress = null) {
            using var unityWebRequest = UnityWebRequest.Get(_url);

            Debug.Log(_url);

            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(_headerAccessToken)) {
                unityWebRequest.SetRequestHeader("Authorization", _headerAccessToken);
            }

            var operation = unityWebRequest.SendWebRequest();

            while (!operation.isDone) {
                Progress?.Invoke(operation.progress);
                await Task.Yield();
            }

            Debug.Log(unityWebRequest);
            Debug.Log(unityWebRequest.downloadHandler);
            Debug.Log(unityWebRequest.downloadHandler.text);

            if (unityWebRequest.result == UnityWebRequest.Result.Success) {
                Debug.Log($"Success: {unityWebRequest.downloadHandler.text}");
                Success?.Invoke(unityWebRequest.downloadHandler.text);
            } else {
                Debug.LogError($"Failed: {unityWebRequest.error}");
                Fail?.Invoke(unityWebRequest.error);
            }
            //IRequestDealer requestOperation = new RequestDealer();
            //requestOperation.Send(unityWebRequest, Success, Fail);
        }
    }
}
