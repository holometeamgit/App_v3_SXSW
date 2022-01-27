using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem.UI {
    /// <summary>
    /// Close ARMessage Btn
    /// </summary>
    public class CloseARMessagesBtn : MonoBehaviour, IARMsgDataView {

        private ARMsgJSON.Data currentData;

        /// <summary>
        /// Close ARMessage
        /// </summary>
        public void Close() {

            WarningConstructor.ActivateDoubleButton("Before you go...",
           "If you exit you could lose your AR message if you don't share the link.",
           "Copy link and exit", "Return",
           () => {
               GUIUtility.systemCopyBuffer = currentData.share_link;
               ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
               ARenaConstructor.onDeactivate?.Invoke();
               MenuConstructor.OnActivated?.Invoke(true);
               HomeScreenConstructor.OnActivated?.Invoke(true);
           },
           null,
           false);
        }

        public void Init(ARMsgJSON.Data data) {
            currentData = data;
        }
    }
}