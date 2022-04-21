using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Mover
/// </summary>
public class Mover : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private RectTransform _rect;

    [SerializeField]
    private float _frequency = 0.02f;

    [SerializeField]
    private float _speed = 1f;

    private Coroutine _enumerator;

    private bool active = false;

    private const string MOVE_KEY = "Moving";
    private const float MOVE_CEIL = 0.5f;

    public event Action onStartMoving;
    public event Action<bool> onEndMoving;

    public void OnBeginDrag(PointerEventData eventData) {
        Cancel();
    }

    public void OnDrag(PointerEventData eventData) {
        CurrentStatus = eventData.position.y / _rect.sizeDelta.y;
    }

    public void OnEndDrag(PointerEventData eventData) {
        ChangeValue(CurrentStatus > MOVE_CEIL ? 1f : 0f);
    }

    private void ChangeValue(float endValue) {
        Cancel();
        _enumerator = StartCoroutine(Moving(endValue));
    }

    private void Cancel() {
        if (_enumerator != null) {
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

    protected float CurrentStatus {
        get {
            return _animator.GetFloat(MOVE_KEY);
        }
        set {
            _animator.SetFloat(MOVE_KEY, value);
        }
    }

    private IEnumerator Moving(float endValue) {
        onStartMoving?.Invoke();
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
