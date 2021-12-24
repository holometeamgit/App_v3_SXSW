using Beem.Firebase.DynamicLink;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for prerecorded video
    /// </summary>

    public class SharePrerecordedBtn : MonoBehaviour, IStreamDataView {

        private StreamJsonData.Data _streamData = default;

        public void Init(StreamJsonData.Data streamData) {
            _streamData = streamData;
        }

        /// <summary>
        /// Share prerecorded video
        /// </summary>
        public void Share() {
            if (_streamData != null) {
                StreamCallBacks.onShareStreamLinkByData?.Invoke(_streamData);
            }
        }
    }
}
