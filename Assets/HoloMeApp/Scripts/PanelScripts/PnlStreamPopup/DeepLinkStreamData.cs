using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeepLink Stream data
/// </summary>
public class DeepLinkStreamData {

    private string _title;
    private string _description;
    private IData _data;
    private bool _online;
    private bool _closeBtn;
    private bool _shareBtn;

    public DeepLinkStreamData(string title, string description, IData data, bool online, bool closeBtn, bool shareBtn) {
        _title = title;
        _description = description;
        _data = data;
        _online = online;
        _closeBtn = closeBtn;
        _shareBtn = shareBtn;
    }

    public IData Data {
        get {
            return _data;
        }
    }

    public string Title {
        get {
            return _title;
        }
    }

    public string Description {
        get {
            return _description;
        }
    }

    public bool Online {
        get {
            return _online;
        }
    }

    public bool CloseBtn {
        get {
            return _closeBtn;
        }
    }

    public bool ShareBtn {
        get {
            return _shareBtn;
        }
    }
}
