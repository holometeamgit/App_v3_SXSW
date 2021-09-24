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

        public void Send(Action<string> Success = null, Action<string> Fail = null, Action<float> Progress = null) {
            string json = JsonUtility.ToJson(_body);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using var unityWebRequest = UnityWebRequest.Put(_url, bodyRaw);

            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(_headerAccessToken)) {
                unityWebRequest.SetRequestHeader("Authorization", _headerAccessToken);
            }

            IRequestDealer requestOperation = new RequestDealer();
            requestOperation.Send(unityWebRequest, Success, Fail, Progress);
        }
    }
}
