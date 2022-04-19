using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for StreamView
    /// </summary>

    public interface IStreamDataView {
        /// <summary>
        /// Init StreamDataView
        /// </summary>
        /// <param name="data"></param>
        void Init(StreamJsonData.Data data);
    }
}
