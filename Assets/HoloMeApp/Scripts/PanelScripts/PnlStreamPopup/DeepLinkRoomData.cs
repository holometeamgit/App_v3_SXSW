using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deep Link Room Data
/// </summary>
public struct DeepLinkRoomData {
    public enum Settings {
        Online,
        Offline,
        Ended,
        NotExist
    }

    private string _text;

    public string GetText() {
        return _text;
    }

    private Settings _settings;

    public Settings GetSettings() {
        return _settings;
    }
    public DeepLinkRoomData(string text, Settings settings) {
        _text = text;
        _settings = settings;
    }

}
