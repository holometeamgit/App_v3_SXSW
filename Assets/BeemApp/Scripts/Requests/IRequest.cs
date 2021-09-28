using UnityEngine.Networking;

namespace Beem.Utility.Requests {

    /// <summary>
    /// Request
    /// </summary>
    public interface IRequest {
        UnityWebRequest Request { get; }
    }
}
