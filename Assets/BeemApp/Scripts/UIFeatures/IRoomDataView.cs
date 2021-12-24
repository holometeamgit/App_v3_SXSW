using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for RoomView
    /// </summary>

    public interface IRoomDataView {
        /// <summary>
        /// Init RoomDataView
        /// </summary>
        /// <param name="streamData"></param>
        void Init(RoomJsonData data);
    }
}
