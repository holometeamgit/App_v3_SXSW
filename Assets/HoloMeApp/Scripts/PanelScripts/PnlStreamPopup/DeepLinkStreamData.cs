using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DeepLink Stream data
/// </summary>
public class DeepLinkStreamData {

    public enum DeepLinkPopup {
        Online,
        Offline,
        NoLongerLive
    }

    private string _title;
    private string _description;
    private IData _data;
    private bool _online;
    private bool _closeBtn;
    private bool _shareBtn;

    private const string NO_LONGER_LIVE_TITLE = "<color =#{0}>{1}</color> is no longer live";
    private const string NO_LONGER_LIVE_DESCRIPTION = "";
    private const string ONLINE_TITLE = "<color=#{0}>{1}</color>’s is now online";
    private const string ONLINE_DESCRIPTION = "";
    private const string OFFLINE_TITLE = "<color=#{0}>{1}</color>’s is currently offline";
    private const string OFFLINE_DESCRIPTION = "This page will refresh automatically\nwhen they go live";

    public DeepLinkStreamData(DeepLinkPopup deepLinkPopup, IData data) {

        _data = data;
        _online = deepLinkPopup == DeepLinkPopup.Online;
        _closeBtn = deepLinkPopup == DeepLinkPopup.NoLongerLive;
        _shareBtn = deepLinkPopup == DeepLinkPopup.Offline && (data is RoomJsonData);

        switch (deepLinkPopup) {
            case DeepLinkPopup.NoLongerLive:
                _title = NO_LONGER_LIVE_TITLE;
                _description = NO_LONGER_LIVE_DESCRIPTION;
                break;
            case DeepLinkPopup.Online:
                _title = ONLINE_TITLE;
                _description = ONLINE_DESCRIPTION;
                break;
            case DeepLinkPopup.Offline:
                _title = OFFLINE_TITLE;
                _description = OFFLINE_DESCRIPTION;
                break;
        }
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
