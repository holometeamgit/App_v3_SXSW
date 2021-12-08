using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlashScreenshot : MonoBehaviour {

    public UnityEvent OnMakeEventInFlash;
    public UnityEvent OnDone;

    private float _defaultBrightness;
    private bool _underControll;

    public void MakeEventInFlash() {
        OnMakeEventInFlash?.Invoke();
    }

    public void InvokeDone() {
        OnDone?.Invoke();
    }

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
