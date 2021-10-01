using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SwipePopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    [SerializeField]
    AnimationCurve _animationCurve;
    [SerializeField]
    private bool _horizontal;
    [SerializeField]
    private bool _vertical = true;
    [SerializeField]
    RectTransform _swipeObjectTransform;
    [SerializeField]
    CanvasGroup _canvasGroup;
    [Range(0.001f, 20)]
    [SerializeField] float _time = 2;

    private Vector3 _initialPosition;
    private bool _swipeLeft;
    private bool _swipeDown;
    private Vector2 _minMovedDistance;
    private bool _needHorizontalMoving;
    private bool _needVerticalMoving;

    private const float _coefficient = 0.123456789f;
    private const float _coefficientHiding = 1.21f;

    public void UpdateMinMovedDistance() {
        _minMovedDistance = new Vector2(_swipeObjectTransform.sizeDelta.x, _swipeObjectTransform.sizeDelta.y) * _coefficient;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.localPosition = new Vector2(
            transform.localPosition.x + (_horizontal ? eventData.delta.x : 0)
            , transform.localPosition.y + (_vertical ? eventData.delta.y : 0));
    }

    public void OnBeginDrag(PointerEventData eventData) {
        _initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        float distanceMovedX = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        float distanceMovedY = Mathf.Abs(transform.localPosition.y - _initialPosition.y);

        _needHorizontalMoving = !(distanceMovedX < _minMovedDistance.x);
        _needVerticalMoving = !(distanceMovedY < _minMovedDistance.y);

        if (!_needHorizontalMoving && !_needVerticalMoving) {
            transform.localPosition = _initialPosition;
        } else {
            if (transform.localPosition.x > _initialPosition.x) {
                _swipeLeft = false;

            } else {
                _swipeLeft = true;
            }

            if (transform.localPosition.y > _initialPosition.y) {
                _swipeDown = false;

            } else {
                _swipeDown = true;
            }

            StartCoroutine(MovedCard());
        }
    }

    private void OnEnable() {
        UpdateMinMovedDistance();
    }

    private IEnumerator MovedCard() {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        while (_canvasGroup.alpha > 0) {
            time += (Time.deltaTime / _time);

            transform.localPosition = Vector3.Slerp(startPosition,
                new Vector3(
                    _needHorizontalMoving ? ((_swipeLeft ? -Screen.width : Screen.width)): transform.localPosition.x,
                    _needVerticalMoving ? ((_swipeDown ? -Screen.height : Screen.height)) : transform.localPosition.y,
                    0)
                    , Mathf.Clamp01(_animationCurve.Evaluate(time)));
            _canvasGroup.alpha = Mathf.SmoothStep(1, 0, time* _coefficientHiding);
            yield return null;
        }
    }
}
