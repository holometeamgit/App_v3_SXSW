using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// SuccessOptionsData
/// </summary>
[Serializable]
public class SuccessOptionsData {
    private string _title;

    public string Title {
        get {
            return _title;
        }
    }

    private string _description;

    public string Description {
        get {
            return _description;
        }
    }

    private Action _backEvent;

    public Action BackEvent {
        get {
            return _backEvent;
        }
    }

    public SuccessOptionsData(string title, string description, Action backEvent) {
        _title = title;
        _description = description;
        _backEvent = backEvent;
    }
}
