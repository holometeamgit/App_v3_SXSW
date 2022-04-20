using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimParameterChanger : MonoBehaviour, IScrollHandler, IDragHandler, IBeginDragHandler {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float speed;

    private bool isOpened = false;

    public async void OnClick() {
        isOpened = !isOpened;

        float endValue = isOpened ? 1 : 0;
        float startValue = animator.GetFloat("MotionT");
        float currentValue = animator.GetFloat("MotionT");

        while (currentValue != endValue) {
            currentValue += (endValue - startValue) / 100f;
            animator.SetFloat("MotionT", currentValue);
            await Task.Yield();
        }
    }

    public void Change(float val) {
        animator.SetFloat("MotionT", Mathf.Clamp01(val));
    }

    public void OnScroll(PointerEventData eventData) {
        Debug.Log($"eventData.scrollDelta = {eventData.scrollDelta}");
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 diff = (eventData.position - position);
        diff.Normalize();
        currentVal += diff.y * Time.deltaTime * speed;
        Debug.Log($"eventData.delta = {currentVal}");
        Debug.Log($"eventData.delta = {eventData.delta}");
        //animator.SetFloat("MotionT", Mathf.Clamp01(currentVal));
    }

    Vector2 position;
    float currentVal;

    public void OnBeginDrag(PointerEventData eventData) {
        position = eventData.position;
        currentVal = animator.GetFloat("MotionT");
    }
}
