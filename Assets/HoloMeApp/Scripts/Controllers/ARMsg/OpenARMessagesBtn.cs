using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Open Button for AR-Messages
    /// </summary>

    public class OpenARMessagesBtn : MonoBehaviour, IARMsgDataView {

        private ARMsgJSON.Data _arMsgData = default;

        public void Init(ARMsgJSON.Data arMsgData) {
            _arMsgData = arMsgData;
        }

        /// <summary>
        /// Open AR Messages
        /// </summary>
        public void Open() {
            CallBacks.OnActivatedARena?.Invoke(_arMsgData);
            CallBacks.OnActivated?.Invoke(false);
        }
    }
}
