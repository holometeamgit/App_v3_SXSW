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

        public UnityWebRequest Request {
            get {
                UnityWebRequest webRequest = UnityWebRequest.Put(_url, Encoding.UTF8.GetBytes(JsonUtility.ToJson(_body)));
                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(_headerAccessToken)) {
                    webRequest.SetRequestHeader("Authorization", _headerAccessToken);
                }

                return webRequest;
            }
        }

        /// <summary>
        /// Send Request
        /// </summary>

        public void Send(Action<string> Success = null, Action<string> Fail = null) {
            IRequestSender requestSender = new RequestSender();
            requestSender.Send(Request, Success, Fail);
        }

    }
}
