

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Beem.ARMsg;

/// <summary>
/// CircleButtonWithTimer. UI Circle like timer
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class CircleButtonWithTimer : MonoBehaviour {

    [SerializeField]
    private Image _invertCountdown, _countdown;
    [SerializeField]
    private float delayTimer = 2;
    [SerializeField]
    private Counter _counter;
    private bool _pressed;
    private float _maxRecordingTime = 5f; // seconds

    public UnityEvent onStop;

    /// <summary>
    /// SetTimerScale
    /// </summary>
    /// <param name="value">seconds</param>
    public void SetMaxRecordingTime(int value) {
        _maxRecordingTime = value;
        if(_counter != null)
            _counter.SetCounterTime(value);
    }

    /// <summary>
    /// start animation
    /// </summary>
    public void StartAnimation() {
        Reset();
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// RequestStop
    /// </summary>
    public void RequestStop() {
        _pressed = true;
    }

    private void Reset() {
        // Reset fill amounts
        if (_invertCountdown)
            _invertCountdown.fillAmount = 1.0f;
        if (_countdown)
            _countdown.fillAmount = 0.0f;
    }

    private void OnDisable() {
        // Reset
        Reset();
    }

    private IEnumerator Countdown() {
        _pressed = false;

        // Animate the countdown
        yield return new WaitForSeconds(delayTimer);
        float startTime = Time.time, ratio = 0f;

        while (!_pressed && (ratio = (Time.time - startTime) / _maxRecordingTime) < 1.0f) {
            if (_countdown)
                _countdown.fillAmount = ratio;
            if (_invertCountdown)
                _invertCountdown.fillAmount = 1f - ratio;
            yield return null;
        }
        // Stop recording
        onStop?.Invoke();
    }
}
