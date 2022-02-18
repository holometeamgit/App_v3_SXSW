using DynamicScrollRect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMsgScrollItem : ScrollItemData {

    private ARMsgJSON.Data _data;

    public ARMsgJSON.Data Data {
        get {
            return _data;
        }
    }

    public ARMsgScrollItem(int index) : base(index) {

    }

    public void Init(ARMsgJSON.Data data) {
        _data = data;
    }
}
