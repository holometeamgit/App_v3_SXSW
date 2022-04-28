using Mopsicus.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mover
/// </summary>
public class Mover : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _frequency = 0.02f;

    [SerializeField]
    private float _speed = 1f;

    public event Action<bool> onStartMoving;
    public event Action<bool> onEndMoving;


    private Coroutine _enumerator;
    private CanvasScaler _canvasScaler;
    private bool active = false;

    private float _defaultHeight = 1520f;
    private const float MOVE_CEIL = 0.3f;

    private bool isDragging;
    private float currentStatus;

    public bool IsDragging {
        get {
            return isDragging;
        }
    }

    private void OnEnable() {
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        _defaultHeight = _rect.rect.height;
        InputFieldBtn.OnShowKeyboard += OnInputField;
    }

    private void OnDisable() {
        InputFieldBtn.OnShowKeyboard -= OnInputField;
        Cancel();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Cancel();
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        CurrentStatus = Mathf.Clamp01(eventData.position.y / (Screen.height * (_rect.sizeDelta.y / _canvasScaler.referenceResolution.y)));
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

    private void OnInputField(bool isShown, int height) {
        Debug.LogError($"OnMobileKeyboard {isShown}, height = {height}");

        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, _defaultHeight + (isShown ? height : 0));

        CurrentStatus = 1f;
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
            _rect.anchoredPosition = Vector2.up * (currentStatus - 1) * _rect.rect.height;
            _canvasGroup.alpha = currentStatus;
            _canvasGroup.blocksRaycasts = currentStatus > MOVE_CEIL;
            _canvasGroup.interactable = currentStatus > MOVE_CEIL;
        }
    }

    private IEnumerator Moving(float endValue) {
        onStartMoving?.Invoke(endValue > MOVE_CEIL);
        float startValue = CurrentStatus;
        float currentValue = startValue;
        while (Mathf.Abs(currentValue - endValue) > 0.01f) {
            currentValue += (endValue - startValue) * _frequency * _speed;
            CurrentStatus = currentValue;
            yield return new WaitForSeconds(_frequency);
        }
        currentValue = endValue;
        CurrentStatus = currentValue;
        onEndMoving?.Invoke(endValue > MOVE_CEIL);
    }


}
