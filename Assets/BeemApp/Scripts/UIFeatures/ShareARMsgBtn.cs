using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for AR-Messages
    /// </summary>

    public class ShareARMsgBtn : MonoBehaviour, IARMsgDataView {

        private ARMsgJSON.Data _data = default;

        private ShareLinkController _shareController = new ShareLinkController();

        public void Init(ARMsgJSON.Data data) {
            _data = data;
        }

        /// <summary>
        /// Share AR Messages
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_data.share_link.ToString())) {
                _shareController.ShareLink(_data.share_link);
            }
        }
    }
}
