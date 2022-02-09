using Beem.ARMsg;
using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.UI {

    /// <summary>
    /// Open Button for AR-Messages
    /// </summary>

    public class OpenARMessagesBtn : MonoBehaviour, IARMsgDataView {

        private ARMsgJSON.Data _arMsgData = default;
        private ContentPlayer _contentPlayer;

        [Inject]
        public void Construct(UserWebManager userWebManager) {
            _contentPlayer = new ContentPlayer(userWebManager);
        }

        public void Init(ARMsgJSON.Data arMsgData) {
            _arMsgData = arMsgData;
        }

        /// <summary>
        /// Open AR Messages
        /// </summary>
        public void Open() {
            _contentPlayer.PlayARMessage(_arMsgData);
            ARMsgRecordConstructor.OnHide?.Invoke();
            CallBacks.OnCancelAllARMsgActions?.Invoke();
        }
    }
}
