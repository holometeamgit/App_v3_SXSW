using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for WebRequestHandler
    /// </summary>

    public interface IWebRequestHandlerView {
        /// <summary>
        /// Init WebRequestHandler
        /// </summary>
        /// <param name="webRequestHandler"></param>
        void Init(WebRequestHandler webRequestHandler);
    }
}
