using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {

    /// <summary>
    /// Put Request
    /// </summary>
    public class PutRequest<T> : IRequest {

        private string _url;
        private string _headerAccessToken;
        private T _body;

        public PutRequest(string url, T body, string headerAccessToken = null) {
            _url = url;
            _headerAccessToken = headerAccessToken;
            _body = body;
        }

        /// <summary>
        /// Send Request
        /// </summary>

        public async void Send(Action<string> Success = null, Action<string> Fail = null) {
            string json = JsonUtility.ToJson(_body);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            Debug.Log(_url);

            Debug.Log(json);

            using var unityWebRequest = UnityWebRequest.Put(_url, bodyRaw);

            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(_headerAccessToken)) {
                unityWebRequest.SetRequestHeader("Authorization", _headerAccessToken);
            }

            UnityWebRequest.Result result = await unityWebRequest.SendWebRequest();

            if (result == UnityWebRequest.Result.Success) {
                Debug.Log($"Success: {unityWebRequest.downloadHandler.text}");
                Success?.Invoke(unityWebRequest.downloadHandler.text);
            } else {
                Debug.LogError($"Failed: {unityWebRequest.error}");
                Fail?.Invoke(unityWebRequest.error);
            }
        }

    }
}
