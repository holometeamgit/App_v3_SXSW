using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {
    /// <summary>
    /// Interface for ARMessageView
    /// </summary>

    public interface IARMsgDataView {
        /// <summary>
        /// Init ARMsgDataView
        /// </summary>
        /// <param name="arMsgJSON"></param>
        void Init(ARMsgJSON.Data arMsgJSON);
    }
}
