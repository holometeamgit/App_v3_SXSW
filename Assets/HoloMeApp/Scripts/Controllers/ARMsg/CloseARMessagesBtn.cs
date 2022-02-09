using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowManager.Extenject;

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
            GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Copy link and exit", CopyAndExit);
            GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Return", null);
            GeneralPopUpData data = new GeneralPopUpData("Before you go...", "If you exit you could lose your AR message if you don't share the link.", closeButton, funcButton);

            WarningConstructor.OnShow?.Invoke(data);
        }

        private void CopyAndExit() {
            GUIUtility.systemCopyBuffer = currentData.share_link;
            ARMsgARenaConstructor.OnHide?.Invoke();
            ARenaConstructor.onDeactivate?.Invoke();
            HomeConstructor.OnActivated?.Invoke(true);
            BottomMenuConstructor.OnActivated?.Invoke(true);
        }

        public void Init(ARMsgJSON.Data data) {
            currentData = data;
        }
    }
}