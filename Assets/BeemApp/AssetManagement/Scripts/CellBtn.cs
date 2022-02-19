using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBtn : MonoBehaviour, IARMsgDataView {

    private ARMsgJSON.Data _arMsgData = default;

    public void Init(ARMsgJSON.Data arMsgData) {
        _arMsgData = arMsgData;
    }

    /// <summary>
    /// Open AR Messages
    /// </summary>
    public void Open() {
        StreamCallBacks.onPlayARMessage?.Invoke(_arMsgData);
        GalleryConstructor.OnHide?.Invoke();
    }
}
