using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for AR-Messages
    /// </summary>

    public class ShareARMessagesBtn : MonoBehaviour, IARMsgDataView {

        private ARMsgJSON.Data _arMsgData = default;

        public void Init(ARMsgJSON.Data arMsgData) {
            _arMsgData = arMsgData;
        }

        /// <summary>
        /// Share AR Messages
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_arMsgData.share_link.ToString())) {
                DynamicLinksCallBacks.onShareLink?.Invoke(new Uri(_arMsgData.share_link));
            }
        }
    }
}
