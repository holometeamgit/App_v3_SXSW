using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect), typeof(CanvasGroup))]
public class ScrollSnap : UIBehaviour, IDragHandler, IEndDragHandler {
    [SerializeField] public int startingIndex = 0;
    [SerializeField] public bool wrapAround = false;
    [SerializeField] public float lerpTimeMilliSeconds = 200f;
    [SerializeField] public float triggerPercent = 5f;
    [Range(0f, 10f)] public float triggerAcceleration = 1f;

    [Serializable]
    public class OnProgressEvent : UnityEvent<float, int> { }
    public OnProgressEvent onProgress;

    public class OnLerpCompleteEvent : UnityEvent { }
    public OnLerpCompleteEvent onLerpComplete;

    public class OnReleaseEvent : UnityEvent<int> { }
    public OnReleaseEvent onRelease;

    [SerializeField] float cellWidth;
    [SerializeField] UnityEvent[] OnIndexSnapEvent;

    private int actualIndex;
    private int cellIndex;
    private ScrollRect scrollRect;
    private CanvasGroup canvasGroup;
    private RectTransform content;
    private bool indexChangeTriggered = false;
    private bool isLerping = false;
    private float lerpStartedAt;
    private Vector2 releasedPosition;
    private Vector2 targetPosition;

    public int CurrentIndex {
        get {
            int count = LayoutElementCount();
            int mod = actualIndex % count;
            return mod >= 0 ? mod : count + mod;
        }
    }

    /// <summary>
    /// Push layout element to interface
    /// </summary>
    public void PushLayoutElement(LayoutElement element) {
        element.transform.SetParent(content.transform, false);
        SetContentSize(LayoutElementCount());
    }

    /// <summary>
    /// Pop layout element to interface
    /// </summary>
    public void PopLayoutElement() {
        LayoutElement[] elements = content.GetComponentsInChildren<LayoutElement>();
        Destroy(elements[elements.Length - 1].gameObject);
        SetContentSize(LayoutElementCount() - 1);
        if (cellIndex == CalculateMaxIndex()) {
            cellIndex -= 1;
        }
    }

    /// <summary>
    /// Unshift layout element and set as first sibling
    /// </summary>
    public void UnshiftLayoutElement(LayoutElement element) {
        cellIndex += 1;
        element.transform.SetParent(content.transform, false);
        element.transform.SetAsFirstSibling();
        SetContentSize(LayoutElementCount());
        content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellWidth, content.anchoredPosition.y);
        onProgress?.Invoke(-(content.anchoredPosition.x - cellWidth) / cellWidth, LayoutElementCount());
    }

    /// <summary>
    /// check index change on drag
    /// </summary>
    public void OnDrag(PointerEventData data) {
        float dx = data.delta.x;
        float dt = Time.deltaTime * 1000f;
        float acceleration = Mathf.Abs(dx / dt);
        if (acceleration > triggerAcceleration && !float.IsPositiveInfinity(acceleration)) {
            indexChangeTriggered = true;
        }
    }

    /// <summary>
    /// check index change or start lerping
    /// </summary>
    public void OnEndDrag(PointerEventData data) {
        if (IndexShouldChangeFromDrag(data)) {
            int direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount(data));
        } else {
            StartLerping();
        }
    }

    protected override void Awake() {
        base.Awake();
        actualIndex = startingIndex;
        cellIndex = startingIndex;
        this.onLerpComplete = new OnLerpCompleteEvent();
        this.onRelease = new OnReleaseEvent();
        this.scrollRect = GetComponent<ScrollRect>();
        this.canvasGroup = GetComponent<CanvasGroup>();
        this.content = scrollRect.content;
        content.anchoredPosition = new Vector2(-cellWidth * cellIndex, content.anchoredPosition.y);
        onProgress?.Invoke((cellWidth * cellIndex) / cellWidth, LayoutElementCount());
        int count = LayoutElementCount();
        SetContentSize(count);

        if (startingIndex < count) {
            MoveToIndex(startingIndex);
        }
    }

    protected override void Start() {
        base.Start();
        content.offsetMin = new Vector2(content.offsetMin.x, 0);
    }

    private void ShiftLayoutElement() {
        Destroy(GetComponentInChildren<LayoutElement>().gameObject);
        SetContentSize(LayoutElementCount() - 1);
        cellIndex -= 1;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellWidth, content.anchoredPosition.y);
    }

    private int LayoutElementCount() {
        return content.GetComponentsInChildren<LayoutElement>(false)
            .Count(e => e.transform.parent == content);
    }

    /// /// <summary>
    /// Snaps to next index
    /// </summary>
    public void SnapToNext() {
        SnapToIndex(cellIndex + 1);
    }

    /// <summary>
    /// Snaps to previous index
    /// </summary>
    public void SnapToPrev() {
        SnapToIndex(cellIndex - 1);
    }

    /// <summary>
    /// Resets scroll view to first index
    /// </summary>
    public void ResetToFirst() {
        SnapToIndex(0);
    }

    private void SnapToIndex(int newCellIndex) {
        int maxIndex = CalculateMaxIndex();
        if (wrapAround && maxIndex > 0) {
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
            onLerpComplete.AddListener(WrapElementAround);
        } else {
            newCellIndex = Mathf.Clamp(newCellIndex, 0, maxIndex);
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
        }
        onRelease.Invoke(cellIndex);
        if (OnIndexSnapEvent != null && OnIndexSnapEvent.Length - 1 >= cellIndex) {
            OnIndexSnapEvent?[cellIndex]?.Invoke();
        }
        StartLerping();
    }

    private void MoveToIndex(int newCellIndex) {
        int maxIndex = CalculateMaxIndex();
        if (newCellIndex >= 0 && newCellIndex <= maxIndex) {
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
        }
        onRelease.Invoke(cellIndex);
        content.anchoredPosition = CalculateTargetPoisition(cellIndex);
    }

    private void LateUpdate() {
        if (isLerping) {
            LerpToElement();
            if (ShouldStopLerping()) {
                isLerping = false;
                canvasGroup.blocksRaycasts = true;
                onLerpComplete.Invoke();
                onLerpComplete.RemoveListener(WrapElementAround);
            }
        }
    }

    private void StartLerping() {
        releasedPosition = content.anchoredPosition;
        targetPosition = CalculateTargetPoisition(cellIndex);
        lerpStartedAt = Time.time;
        canvasGroup.blocksRaycasts = false;
        isLerping = true;
    }

    private int CalculateMaxIndex() {
        int cellPerFrame = Mathf.FloorToInt(scrollRect.GetComponent<RectTransform>().rect.size.x / cellWidth);
        return LayoutElementCount() - cellPerFrame;
    }

    private bool IndexShouldChangeFromDrag(PointerEventData data) {
        // acceleration was above threshold
        if (indexChangeTriggered) {
            indexChangeTriggered = false;
            return true;
        }
        // dragged beyond trigger threshold
        var offset = scrollRect.content.anchoredPosition.x + cellIndex * cellWidth;
        var normalizedOffset = Mathf.Abs(offset / cellWidth);
        return normalizedOffset * 100f > triggerPercent;
    }

    private int CalculateScrollingAmount(PointerEventData data) {
        var offset = scrollRect.content.anchoredPosition.x + cellIndex * cellWidth;
        var normalizedOffset = Mathf.Abs(offset / cellWidth);
        var skipping = (int)Mathf.Floor(normalizedOffset);
        if (skipping == 0)
            return 1;
        if ((normalizedOffset - skipping) * 100f > triggerPercent) {
            return skipping + 1;
        } else {
            return skipping;
        }
    }

    private void LerpToElement() {
        float t = (Time.time - lerpStartedAt) * 1000f / lerpTimeMilliSeconds;
        float newX = Mathf.Lerp(releasedPosition.x, targetPosition.x, t);
        content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);
        onProgress?.Invoke(-newX / cellWidth, LayoutElementCount());
    }

    private void WrapElementAround() {
        if (cellIndex <= 0) {
            var elements = content.GetComponentsInChildren<LayoutElement>();
            elements[elements.Length - 1].transform.SetAsFirstSibling();
            cellIndex += 1;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellWidth, content.anchoredPosition.y);
            onProgress?.Invoke(-(content.anchoredPosition.x - cellWidth) / cellWidth, LayoutElementCount());
        } else if (cellIndex >= CalculateMaxIndex()) {
            var element = content.GetComponentInChildren<LayoutElement>();
            element.transform.SetAsLastSibling();
            cellIndex -= 1;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellWidth, content.anchoredPosition.y);
            onProgress?.Invoke(-(content.anchoredPosition.x - cellWidth) / cellWidth, LayoutElementCount());
        }
    }

    private void SetContentSize(int elementCount) {
        content.sizeDelta = new Vector2(cellWidth * elementCount, content.rect.height);
    }

    private Vector2 CalculateTargetPoisition(int index) {
        return new Vector2(-cellWidth * index, content.anchoredPosition.y);
    }

    private bool ShouldStopLerping() {
        return Mathf.Abs(content.anchoredPosition.x - targetPosition.x) < 0.001f;
    }
}
