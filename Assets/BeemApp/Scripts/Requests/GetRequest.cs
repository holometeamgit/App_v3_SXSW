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
            Debug.LogError("GetRequest _url = " + _url);
            Debug.LogError("GetRequest _headerAccessToken = " + _headerAccessToken);
        }

        public UnityWebRequest Request {
            get {
                Debug.LogError("Request _url = " + _url);
                Debug.LogError("Request _headerAccessToken = " + _headerAccessToken);
                UnityWebRequest webRequest = UnityWebRequest.Get(_url);
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

            Debug.LogError("Send _url = " + _url);
            Debug.LogError("Send _headerAccessToken = " + _headerAccessToken);

            Debug.LogError("Send Request");
            IRequestSender requestSender = new RequestSender();
            Debug.LogError("RequestSender");
            requestSender.Send(Request, Success, Fail);
        }
    }
}
