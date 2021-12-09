

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Beem.ARMsg;

[RequireComponent(typeof(EventTrigger))]
public class RecordButtonWithTimer : MonoBehaviour, IPointerDownHandler {

    public Image button, countdown;
    public UnityEvent onStop;
    private bool pressed;
    [SerializeField] private const float MaxRecordingTime = 15f; // seconds

    public void StartRecord() {
        CallBacks.OnStartRecord?.Invoke();
    }

    public void StartRecordAnimation() {
        Reset();
        StartCoroutine(Countdown());
    }

    private void Reset() {
        // Reset fill amounts
        if (button)
            button.fillAmount = 1.0f;
        if (countdown)
            countdown.fillAmount = 0.0f;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        pressed = true;
    }

    private IEnumerator Countdown() {
        pressed = false;
        // Animate the countdown
        float startTime = Time.time, ratio = 0f;
        while (!pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f) {
            countdown.fillAmount = ratio;
            button.fillAmount = 1f - ratio;
            yield return null;
        }
        // Reset
        Reset();
        // Stop recording
        onStop?.Invoke();
        CallBacks.OnStopRecord?.Invoke();
    }
}
