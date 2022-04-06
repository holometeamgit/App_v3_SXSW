using DynamicScrollRect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ARMsgScroll Item
/// </summary>
public class ARMsgScrollItem : ScrollItemData {

    private ARMsgJSON.Data _data;
    private UserWebManager _userWebManager;
    private bool _isNew;

    public ARMsgJSON.Data Data {
        get {
            return _data;
        }
    }

    public UserWebManager Manager {
        get {
            return _userWebManager;
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
    public void Init(ARMsgJSON.Data data, UserWebManager userWebManager, bool isNew) {
        _data = data;
        _userWebManager = userWebManager;
        _isNew = isNew;
    }
}
