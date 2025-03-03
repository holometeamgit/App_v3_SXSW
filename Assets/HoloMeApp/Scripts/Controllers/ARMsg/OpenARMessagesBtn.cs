using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Beem.Permissions;
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
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
            ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
            CallBacks.OnCancelAllARMsgActions?.Invoke();
            PnlRecord.CurrentUser = _arMsgData.user;
        }
    }
}
