

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Beem.ARMsg;

[RequireComponent(typeof(EventTrigger))]
public class CircleButtonWithTimer : MonoBehaviour {

    public Image invertCountdown, countdown;
    public UnityEvent onStop;
    private bool pressed;
    [SerializeField] private float MaxRecordingTime = 15f; // seconds
    [SerializeField] private float delayTimer = 2;

    public void StartAnimation() {
        Reset();
        StartCoroutine(Countdown());
    }

    private void Reset() {
        // Reset fill amounts
        if (invertCountdown)
            invertCountdown.fillAmount = 1.0f;
        if (countdown)
            countdown.fillAmount = 0.0f;
    }

    /// <summary>
    /// RequestStop
    /// </summary>
    public void RequestStop() {
        pressed = true;
    }

    private IEnumerator Countdown() {
        pressed = false;

        // Animate the countdown
        yield return new WaitForSeconds(delayTimer);
        float startTime = Time.time, ratio = 0f;

        while (!pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f) {
            if (countdown)
                countdown.fillAmount = ratio;
            if (invertCountdown)
                invertCountdown.fillAmount = 1f - ratio;
            yield return null;
        }
        // Stop recording
        onStop?.Invoke();
    }

    private void OnDisable() {
        // Reset
        Reset();
    }
}
