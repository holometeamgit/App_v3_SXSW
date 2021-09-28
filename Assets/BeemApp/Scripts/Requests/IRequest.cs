using System;

namespace Beem.Utility.Requests {
    /// <summary>
    /// Base Request
    /// </summary>
    public interface IRequest {

        /// <summary>
        /// Send request
        /// </summary>
        /// <param name="Success"></param>
        /// <param name="Fail"></param>
        /// <param name="Progress"></param>
        void Send(Action<string> Success = null, Action<string> Fail = null);
    }
}
