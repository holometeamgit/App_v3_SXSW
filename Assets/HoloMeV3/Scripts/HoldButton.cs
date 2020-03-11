using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Image countdown;
    public UnityEvent onTouchDown, onTouchUp;
    private bool pressed;
    private const float MaxRecordingTime = 15; // seconds

    [SerializeField]
    PermissionGranter permissionGranter;

    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        // Reset fill amounts
        if (countdown) countdown.fillAmount = 0.0f;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!permissionGranter.MicRequestComplete)
        {
            return;
        }
        // Start counting
        StartCoroutine(Countdown());
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        // Reset pressed
        pressed = false;
    }

    private IEnumerator Countdown()
    {
        pressed = true;
        // First wait a short time to make sure it's not a tap
        yield return new WaitForSeconds(0.2f);
        if (!pressed) yield break;
        // Start recording
        if (onTouchDown != null) onTouchDown.Invoke();
        // Animate the countdown
        float startTime = Time.time, ratio = 0f;
        while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f)
        {
            countdown.fillAmount = ratio;
            yield return null;
        }
        // Reset
        Reset();
        // Stop recording
        if (onTouchUp != null) onTouchUp.Invoke();
    }
}
