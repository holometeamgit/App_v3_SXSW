using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Image countdown;
    public UnityEvent onTouchDown, onTouchUp, onRecordTooShort;
    private bool pressed;
    private const float MaxRecordingTime = 15; // seconds
    private const float MinRecordingTime = 2; // seconds
    private const float AccidentTapTime = 0.2f;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    private void Start() {
        Reset();
    }

    private void Reset() {
        // Reset fill amounts
        if (countdown) countdown.fillAmount = 0.0f;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        // Start counting
        StartCoroutine(Countdown());
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        // Reset pressed
        pressed = false;
    }

    private IEnumerator Countdown() {
        pressed = true;

        // First wait a short time to make sure it's not a tap
        yield return new WaitForSeconds(AccidentTapTime);

        if (!pressed) {
            onRecordTooShort?.Invoke(); //Trying to call screenshot path is short pressed here 
            yield break;
        }

        if (!permissionController.CheckMicAccess()) {
            yield break;
        }

        // Start recording
        if (onTouchDown != null) onTouchDown.Invoke();

        // Animate the countdown
        float startTime = Time.time, ratio = 0f;
        while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f) {
            countdown.fillAmount = ratio;
            yield return null;
        }

        Reset();

        if ((Time.time - startTime) < (MinRecordingTime - AccidentTapTime)) {
            onRecordTooShort?.Invoke();
        }

        if (onTouchUp != null) onTouchUp.Invoke();
    }
}
