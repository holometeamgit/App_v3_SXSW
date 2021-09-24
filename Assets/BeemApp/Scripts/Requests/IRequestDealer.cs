using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {
    /// <summary>
    /// Operation for request
    /// </summary>
    public interface IRequestDealer {

        /// <summary>
        /// Send Request
        /// </summary>
        /// <param name="unityWebRequest"></param>
        /// <param name="Success"></param>
        /// <param name="Fail"></param>
        /// <param name="Progress"></param>
        void Send(UnityWebRequest unityWebRequest, Action<string> Success = null, Action<string> Fail = null, Action<float> Progress = null);
    }
}
