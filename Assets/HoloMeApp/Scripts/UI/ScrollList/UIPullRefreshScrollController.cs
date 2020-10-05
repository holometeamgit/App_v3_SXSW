using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPullRefreshScrollController : MonoBehaviour
{
    public Action OnRefresh;

    [SerializeField] private UIRefreshableScroll scrollRect;
    [SerializeField] private float distanceRequiredRefresh = 200;
    private float initialPosition;      
    private float progress;
    private Vector2 stopPosition;
    private bool isPulled;
    private bool isRefreshing;

    public void EndRefreshing() {
        isPulled = false;
        isRefreshing = false;
    }

    private void Start() {
        initialPosition = GetContentAnchoredPosition();
        stopPosition = new Vector2(scrollRect.content.anchoredPosition.x, initialPosition - distanceRequiredRefresh);
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void LateUpdate() {
        if (!isPulled || !isRefreshing) {
            return;
        }

        scrollRect.content.anchoredPosition = stopPosition;
    }

    private void OnScroll(Vector2 normalizedPosition) {
        var distance = initialPosition - GetContentAnchoredPosition();

        if (distance < 0f) {
            return;
        }

        OnPull(distance);
    }

    private void OnPull(float distance) {
        if (isRefreshing && Math.Abs(distance) < 1f) {
            isRefreshing = false;
        }

        if (isPulled && scrollRect.Dragging) {
            return;
        }

        progress = distance / distanceRequiredRefresh;

        if (progress < 1f) {
            return;
        }

        if (scrollRect.Dragging) {
            isPulled = true;
        }

        if (isPulled && !scrollRect.Dragging) {
            isRefreshing = true;
            OnRefresh?.Invoke();
        }

        progress = 0f;
    }

    private float GetContentAnchoredPosition() {
        return scrollRect.content.anchoredPosition.y;
    }
}
