using UnityEngine;

namespace Beem.UI {

    /// <summary>
    /// Share Button for prerecorded data
    /// </summary>

    public class SharePrerecordedBtn : MonoBehaviour, IStreamDataView {

        private StreamJsonData.Data _data = default;

        private ShareLinkController _shareController = new ShareLinkController();

        public void Init(StreamJsonData.Data data) {
            _data = data;
        }

        /// <summary>
        /// Share prerecorded video
        /// </summary>
        public void Share() {

            if (!string.IsNullOrWhiteSpace(_data.share_link.ToString())) {
                _shareController.ShareLink(_data.share_link);
            }
        }
    }
}
