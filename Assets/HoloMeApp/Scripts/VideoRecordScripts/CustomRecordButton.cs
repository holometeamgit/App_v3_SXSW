namespace NatCorder.Examples
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(EventTrigger))]
    public class CustomRecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image button, countdown;
        public UnityEvent onTouchDownShort, onTouchDown, onTouchUp;
        private bool pressed;
        private const float MaxRecordingTime = 26f; // seconds

        private void Start()
        {
            Reset();
        }

        public void StopRecord()
        {
            if (pressed)
            {
                StopAllCoroutines();
                Reset();
                if (onTouchUp != null) onTouchUp.Invoke();
            }
        }

        private void Reset()
        {
            // Reset fill amounts
            //if (button) button.fillAmount = 1.0f;
            if (countdown) countdown.fillAmount = 0.0f;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            // Start counting
            StartCoroutine(Countdown());
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            // Reset pressed
            pressed = false;

            var downScale = new Vector3(1, 1, 1);
            button.transform.localScale = downScale;
            //countdown.transform.localScale = downScale;
        }

        private IEnumerator Countdown()
        {
            pressed = true;
            // First wait a short time to make sure it's not a tap
            yield return new WaitForSeconds(0.25f);
            if (!pressed)
            {
                onTouchDownShort?.Invoke();
                yield break;
            }
            // Start recording
            if (onTouchDown != null) onTouchDown.Invoke();

            var upScale = new Vector3(1.25f, 1.25f, 1.25f);
            button.transform.localScale = upScale;
            //countdown.transform.localScale = upScale;

            // Animate the countdown
            float startTime = Time.time, ratio = 0f;
            while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f)
            {
                countdown.fillAmount = ratio;
                //button.fillAmount = 1f - ratio;
                yield return null;
            }
            // Reset
            Reset();
            // Stop recording
            if (onTouchUp != null) onTouchUp.Invoke();
        }
    }
}