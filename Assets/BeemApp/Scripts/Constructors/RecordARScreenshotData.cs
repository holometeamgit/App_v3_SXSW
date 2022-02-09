using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RecordARScreenshotdata
/// </summary>
public class RecordARScreenshotData {

    public RecordARScreenshotData(Sprite sprite, Texture2D screenshotTexture, string path) {
        _sprite = sprite;
        _screenshotTexture = screenshotTexture;
        _path = path;
    }

    private Sprite _sprite;

    public Sprite GetSprite {
        get {
            return _sprite;
        }
    }

    private Texture2D _screenshotTexture;

    public Texture2D GetScreenshotTexture {
        get {
            return _screenshotTexture;
        }
    }

    private string _path;

    public string GetPath {
        get {
            return _path;
        }
    }
}
