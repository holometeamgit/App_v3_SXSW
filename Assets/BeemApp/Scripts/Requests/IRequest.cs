using System;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {

    /// <summary>
    /// Request
    /// </summary>
    public interface IRequest {
        UnityWebRequest Request { get; }

        void Send(Action<string> Success = null, Action<string> Fail = null);
    }
}
