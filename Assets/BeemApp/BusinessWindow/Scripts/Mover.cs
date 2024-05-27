using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mover
/// </summary>
public class Mover : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public enum Position {
        bottom,
        top,
        left,
        right
    }

    [SerializeField]
    private Position position = Position.bottom;

    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _duration = 0.7f;

    public event Action<bool> onStartMoving;
    public event Action<bool> onEndMoving;


    private Coroutine _enumerator;
    private CanvasScaler _canvasScaler;
    private bool active = false;

    private const float MOVE_CEIL = 0.78f;

    private bool isDragging;
    private float currentStatus;

    public bool IsDragging {
        get {
            return isDragging;
        }
    }

    private void OnEnable() {
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        InitBasePosition();
    }

    private void OnDisable() {
        Cancel();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Cancel();
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        CurrentStatus = Mathf.Clamp01(eventData.position.y / (Screen.height * (_rect.rect.height / _canvasScaler.referenceResolution.y)));
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDragging = false;
        ChangeValue(CurrentStatus > MOVE_CEIL ? 1f : 0f);
    }

    private void ChangeValue(float endValue) {
        Cancel();
        if (gameObject.activeInHierarchy) {
            _enumerator = StartCoroutine(Moving(endValue));
        }
    }

    private void Cancel() {
        if (_enumerator != null) {
            isDragging = false;
            StopCoroutine(_enumerator);
            _enumerator = null;
        }
    }

    /// <summary>
    /// Change State
    /// </summary>
    public void ChangeState() {
        active = !active;
        ChangeValue(active ? 1f : 0f);
    }

    /// <summary>
    /// Change State
    /// </summary>
    public void ChangeState(bool state) {
        active = state;
        ChangeValue(active ? 1f : 0f);
    }

    public float CurrentStatus {
        get {
            return currentStatus;
        }
        set {
            currentStatus = value;
            UpdatePosition();
        }
    }

    /// <summary>
    /// Update Position
    /// </summary>
    public void UpdatePosition() {
        switch (position) {
            case Position.bottom:
                _rect.anchoredPosition = (1 - CurrentStatus) * _rect.rect.height * Vector2.down;
                break;
            case Position.top:
                _rect.anchoredPosition = (1 - CurrentStatus) * _rect.rect.height * Vector2.up;
                break;
            case Position.left:
                _rect.anchoredPosition = (1 - CurrentStatus) * _rect.rect.width * Vector2.left;
                break;
            case Position.right:
                _rect.anchoredPosition = (1 - CurrentStatus) * _rect.rect.width * Vector2.right;
                break;
        }

        _canvasGroup.alpha = CurrentStatus;
        _canvasGroup.blocksRaycasts = CurrentStatus > MOVE_CEIL;
        _canvasGroup.interactable = CurrentStatus > MOVE_CEIL;
    }

    private void InitBasePosition() {
        switch (position) {
            case Position.bottom:
                _rect.pivot = Vector2.right / 2f;
                _rect.anchorMin = Vector2.zero;
                _rect.anchorMax = Vector2.right;
                _rect.anchoredPosition = Vector2.down * _rect.rect.height;
                break;
            case Position.top:
                _rect.pivot = Vector2.one - Vector2.right / 2f;
                _rect.anchorMin = Vector2.up;
                _rect.anchorMax = Vector2.one;
                _rect.anchoredPosition = Vector2.up * _rect.rect.height;
                break;
            case Position.left:
                _rect.pivot = Vector2.up / 2f;
                _rect.anchorMin = Vector2.zero;
                _rect.anchorMax = Vector2.up;
                _rect.anchoredPosition = Vector2.left * _rect.rect.width;
                break;
            case Position.right:
                _rect.pivot = Vector2.one - Vector2.up / 2f;
                _rect.anchorMin = Vector2.right;
                _rect.anchorMax = Vector2.one;
                _rect.anchoredPosition = Vector2.right * _rect.rect.width;
                break;
        }
    }

    private IEnumerator Moving(float endValue) {
        onStartMoving?.Invoke(endValue > MOVE_CEIL);
        float startValue = CurrentStatus;
        float currentValue = startValue;
        float startTime = Time.time;
        float t = (Time.time - startTime) / _duration;
        while (t < 1.0f) {
            t = (Time.time - startTime) / _duration;
            currentValue = Mathf.SmoothStep(startValue, endValue, t);
            CurrentStatus = currentValue;
            yield return new WaitForEndOfFrame(); 
        }
        currentValue = endValue;
        CurrentStatus = currentValue;
        onEndMoving?.Invoke(endValue > MOVE_CEIL);
    }


}
