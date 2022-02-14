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
    private string _user;
    private bool _userCountTxt;
    private bool _closeBtn;
    private bool _enterBtn;

    public DeepLinkRoomData(string title, string description, string user, bool userCountTxt, bool closeBtn, bool enterBtn) {
        _title = title;
        _description = description;
        _user = user;
        _userCountTxt = userCountTxt;
        _closeBtn = closeBtn;
        _enterBtn = enterBtn;
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

    public bool UserCountTxt {
        get {
            return _userCountTxt;
        }
    }

    public string User {
        get {
            return _user;
        }
    }

    public bool CloseBtn {
        get {
            return _closeBtn;
        }
    }

    public bool EnterBtn {
        get {
            return _enterBtn;
        }
    }
}
