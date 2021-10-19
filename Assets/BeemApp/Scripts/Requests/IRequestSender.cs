using System;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {
    /// <summary>
    /// Base Request
    /// </summary>
    public interface IRequestSender {

        /// <summary>
        /// Send request
        /// </summary>
        /// <param name="Success"></param>
        /// <param name="Fail"></param>
        /// <param name="Progress"></param>
        void Send(UnityWebRequest webRequest, Action<string> Success = null, Action<string> Fail = null);
    }
}
