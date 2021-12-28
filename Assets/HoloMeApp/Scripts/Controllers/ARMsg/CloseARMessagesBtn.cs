using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem.UI {
    /// <summary>
    /// Close ARMessage Btn
    /// </summary>
    public class CloseARMessagesBtn : MonoBehaviour {

        /// <summary>
        /// Close ARMessage
        /// </summary>
        public void Close() {
            CallBacks.OnDeactivatedARena?.Invoke();
        }
    }
}