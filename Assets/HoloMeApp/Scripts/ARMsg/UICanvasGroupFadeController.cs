using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Need for updating Canvas group fade
/// </summary>
public class UICanvasGroupFadeController : MonoBehaviour {
    [SerializeField]
    private CircleButtonWithTimer _circleButtonWithTimer;
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    [Range(0, 1)]
    private float _startFadeValue;
    [SerializeField]
    [Range(0, 1)]
    private float _endFadeValue;


    private void Awake() {
        _circleButtonWithTimer.onTimerUpdated += UpdateFade;
    }

    private void UpdateFade(float value) {
        _canvasGroup.alpha = Mathf.Lerp(_startFadeValue, _endFadeValue, value);
    }

    private void OnDestroy() {
        _circleButtonWithTimer.onTimerUpdated -= UpdateFade;
    }
}
