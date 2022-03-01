using DynamicScrollRect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ARMsgScroll Item
/// </summary>
public class ARMsgScrollItem : ScrollItemData {

    private ARMsgJSON.Data _data;
    private bool _isNew;

    public ARMsgJSON.Data Data {
        get {
            return _data;
        }
    }

    public bool IsNew {
        get {
            return _isNew;
        }
    }

    public ARMsgScrollItem(int index) : base(index) {

    }

    /// <summary>
    /// Init data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isNew"></param>
    public void Init(ARMsgJSON.Data data, bool isNew) {
        _data = data;
        _isNew = isNew;
    }
}
