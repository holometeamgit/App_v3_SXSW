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
    private string _id;
    private string _username;
    private string _agoraChannel;
    private bool _online;
    private bool _closeBtn;
    private bool _shareBtn;
    private bool _isRoom;

    public DeepLinkStreamData(bool isRoom, string title, string description, string id, string username, string agoraChannel, bool online, bool closeBtn, bool shareBtn) {
        _isRoom = isRoom;
        _title = title;
        _description = description;
        _id = id;
        _username = username;
        _agoraChannel = agoraChannel;
        _online = online;
        _closeBtn = closeBtn;
        _shareBtn = shareBtn;
    }

    public bool IsRoom {
        get {
            return _isRoom;
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

    public string Id {
        get {
            return _id;
        }
    }

    public string Username {
        get {
            return _username;
        }
    }

    public string AgoraChannel {
        get {
            return _agoraChannel;
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
