using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor for post AR REcord
/// </summary>
public class PostRecordARConstructor : MonoBehaviour {
    [SerializeField]
    private PnlPostRecord pnlPostRecord;

    public static Action<string> OnActivatedVideo = delegate { };
    public static Action<Sprite, Texture2D, string> OnActivatedScreenShot = delegate { };
    public static Action OnDeactivated = delegate { };

    private void OnEnable() {
        OnActivatedVideo += ActivateVideo;
        OnActivatedScreenShot += ActivateScreenshot;
        OnDeactivated += Deactivate;
    }

    private void OnDisable() {
        OnActivatedVideo -= ActivateVideo;
        OnActivatedScreenShot -= ActivateScreenshot;
        OnDeactivated -= Deactivate;
    }

    private void ActivateVideo(string path) {
        pnlPostRecord.ActivatePostVideo(path);
    }

    private void ActivateScreenshot(Sprite sprite, Texture2D screenshotTexture, string lastRecordPath) {
        pnlPostRecord.ActivatePostScreenshot(sprite, screenshotTexture, lastRecordPath);
    }

    private void Deactivate() {
        pnlPostRecord.Deactivate();
    }
}
