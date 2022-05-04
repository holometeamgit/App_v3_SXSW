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
    private BusinessProfileManager _businessProfileManager;
    private WebRequestHandler _webRequestHandler;
    private bool _isNew;

    public ARMsgJSON.Data Data {
        get {
            return _data;
        }
    }

    public UserWebManager UserWebManager {
        get {
            return _userWebManager;
        }
    }

    public BusinessProfileManager BusinessProfileManager {
        get {
            return _businessProfileManager;
        }
    }

    public WebRequestHandler WebRequestHandler {
        get {
            return _webRequestHandler;
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
    public void Init(ARMsgJSON.Data data, UserWebManager userWebManager, BusinessProfileManager businessProfileManager, WebRequestHandler webRequestHandler, bool isNew) {
        _data = data;
        _userWebManager = userWebManager;
        _webRequestHandler = webRequestHandler;
        _businessProfileManager = businessProfileManager;
        _isNew = isNew;
    }
}
