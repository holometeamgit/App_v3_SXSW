using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeepLink Room data
/// </summary>
public class DeepLinkRoomData {
    private string _title;
    private string _description;
    private RoomJsonData _data;
    private bool _online;
    private bool _closeBtn;
    private bool _shareBtn;

    public DeepLinkRoomData(string title, string description, RoomJsonData data, bool online, bool closeBtn, bool shareBtn) {
        _title = title;
        _description = description;
        _data = data;
        _online = online;
        _closeBtn = closeBtn;
        _shareBtn = shareBtn;
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

    public RoomJsonData Data {
        get {
            return _data;
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
