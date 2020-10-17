using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPullRefreshScrollController : MonoBehaviour
{
    public Action OnRefresh;
    public Action OnReachedBottom;

    public bool StopBottomRefreshing { get; set; }

    [SerializeField] private UIRefreshableScroll scrollRect;
    [SerializeField] private float distanceRequiredRefresh = 200;
    [SerializeField] private float distanceReachedBottom = 700;
    [SerializeField] private GameObject topLoadingInfo;
    [SerializeField] private GameObject bottomLoadingInfo;
    [SerializeField] bool withStartRefresh;
    private float initialPosition;      
    private float progress;
    private Vector2 stopPosition;
    private bool isPulled;
    private bool isRefreshing;

    private bool isBottomRefreshing;

RectTransform scrollRectTransform; 

    public void EndRefreshing() {
        RefreshLayout();
        isPulled = false;
        isRefreshing = false;
        isBottomRefreshing = false;
        bottomLoadingInfo.SetActive(false);
        topLoadingInfo.SetActive(true);
        Debug.Log("EndRefreshing");
    }

    private void RefreshLayout() {
        Debug.Log("RefreshLayout");
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
    }

    private void Start() {
        initialPosition = GetContentAnchoredPosition();
        stopPosition = new Vector2(scrollRect.content.anchoredPosition.x, initialPosition - distanceRequiredRefresh);
        scrollRect.onValueChanged.AddListener(OnScroll);
        scrollRect.onValueChanged.AddListener(OnBottomPull);
        scrollRectTransform = scrollRect.gameObject.GetComponent<RectTransform>();

        if (withStartRefresh)
            scrollRect.content.anchoredPosition += Vector2.down * distanceRequiredRefresh;
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

        if (isPulled && scrollRect.Dragging || isBottomRefreshing) {
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
            Debug.Log("OnRefresh");
            OnRefresh?.Invoke();
        }

        progress = 0f;
    }

    private void OnBottomPull(Vector2 normalizedPosition) {
        if (isPulled || isRefreshing || isBottomRefreshing || StopBottomRefreshing) {
            return;
        }

//        Debug.Log("GetBottomAnchoredPosition " + GetBottomAnchoredPosition());

        if (GetBottomAnchoredPosition() > distanceReachedBottom)
            return;

        isBottomRefreshing = true;
        Debug.Log("OnBottomPull");
        OnReachedBottom?.Invoke();
        bottomLoadingInfo.SetActive(true);
        topLoadingInfo.SetActive(false);
    }

    private float GetContentAnchoredPosition() {
        return scrollRect.content.anchoredPosition.y;
    }

    private float GetBottomAnchoredPosition() {
        return scrollRect.content.sizeDelta.y - scrollRect.content.anchoredPosition.y - scrollRectTransform.rect.height;
    }
}
