using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPullRefreshScrollController : MonoBehaviour
{
    public Action OnRefresh;
    public Action OnReachedBottom;

    public bool StopBottomRefreshing { get; set; }

    [SerializeField] private UIRefreshableScroll scrollRect;
    [SerializeField] private float distanceRequiredRefresh = 200;
    [SerializeField] private float distanceAutoRefresh = 200;
    [SerializeField] private float distanceReachedBottom = 700;
    [SerializeField] private GameObject topLoadingInfo;
    [SerializeField] private GameObject bottomLoadingInfo;
    [SerializeField] bool withStartRefresh;

    [SerializeField] private float autoRefreshCooldown = 12;
    float stepAutoRefreshCooldown = 1;
    private float initialPosition;      
    private float progress;
    private Vector2 stopPosition;
    private bool isPulled;
    private bool isRefreshing;

    private bool isBottomRefreshing;

    RectTransform scrollRectTransform; 

    public void RefreshLayout() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
    }

    public void EndRefreshing() {
        scrollRect.enabled = true;
        isPulled = false;
        isRefreshing = false;
        isBottomRefreshing = false;
        bottomLoadingInfo.SetActive(false);
        topLoadingInfo.SetActive(true);
        RefreshElementsPosiotion();
//        Debug.Log("EndRefreshing");
    }

    private void RefreshElementsPosiotion() {
        scrollRect.onValueChanged.Invoke(scrollRect.normalizedPosition);
    }

    private void Start() {
        initialPosition = GetContentAnchoredPosition();
        stopPosition = new Vector2(scrollRect.content.anchoredPosition.x, initialPosition - distanceRequiredRefresh);
        scrollRect.onValueChanged.AddListener(OnScroll);
        scrollRect.onValueChanged.AddListener(OnBottomPull);
        scrollRectTransform = scrollRect.gameObject.GetComponent<RectTransform>();

        if (withStartRefresh) {
            Refresh();
        }
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

        if (IsOnPullBusy()) {
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
            Refresh();
        }

        progress = 0f;
    }

    private void OnBottomPull(Vector2 normalizedPosition) {
        if (isPulled || isRefreshing || isBottomRefreshing || StopBottomRefreshing) {
            return;
        }

        if (GetBottomAnchoredPosition() > distanceReachedBottom)
            return;

//        Debug.Log("GetBottomAnchoredPosition " + GetBottomAnchoredPosition());

        isBottomRefreshing = true;
        bottomLoadingInfo.SetActive(true);
        topLoadingInfo.SetActive(false);
//        Debug.Log("OnBottomPull");
        OnReachedBottom?.Invoke();
    }

    private float GetContentAnchoredPosition() {
        return scrollRect.content.anchoredPosition.y;
    }

    private float GetBottomAnchoredPosition() {
        return scrollRect.content.sizeDelta.y - scrollRect.content.anchoredPosition.y - scrollRectTransform.rect.height;
    }

    private bool IsOnPullBusy() {
        return isPulled && scrollRect.Dragging || isBottomRefreshing;
    }

    private void Refresh() {
        isRefreshing = true;
        scrollRect.enabled = false;
        isBottomRefreshing = true;

        OnRefresh?.Invoke();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private void OnEnable() {
        scrollRect.enabled = true;
        StartCoroutine(Autorefreshing());
    }

    IEnumerator Autorefreshing() {
        float timeСountertime = autoRefreshCooldown; 
        while (true) {
            yield return new WaitForSeconds(stepAutoRefreshCooldown);
            if (scrollRect.content.anchoredPosition.y > distanceAutoRefresh ||
                IsOnPullBusy()) {
                timeСountertime = autoRefreshCooldown;
            }

            timeСountertime -= stepAutoRefreshCooldown;

            if(timeСountertime < 0) {
                timeСountertime = autoRefreshCooldown;

                Refresh();
            }
        }
    }
}
