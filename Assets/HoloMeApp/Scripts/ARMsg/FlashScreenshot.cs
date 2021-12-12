using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FlashScreenshot. Make a flash on screen and request event at the end
/// </summary>
public class FlashScreenshot : MonoBehaviour {

    public UnityEvent OnMakeEventInFlash;
    public UnityEvent OnDone;

    private float _defaultBrightness;
    private bool _underControll;

    /// <summary>
    /// Invoke flash event 
    /// </summary>
    public void MakeEventInFlash() {
        OnMakeEventInFlash?.Invoke();
    }

    /// <summary>
    /// Invoke InvokeDone event 
    /// </summary>
    public void InvokeDone() {
        OnDone?.Invoke();
    }

    /// <summary>
    /// ControlScreenBrightness
    /// </summary>
    public void ControlScreenBrightness() {
        _defaultBrightness = Screen.brightness;
        _underControll = true;
    }

    private void Update() {
        if (_underControll) {
            Screen.brightness = Mathf.Lerp(Screen.brightness, 1, 0.1f);
        }
    }

    private void OnDisable() {
        _underControll = false;
        Screen.brightness = _defaultBrightness;
    }
}
