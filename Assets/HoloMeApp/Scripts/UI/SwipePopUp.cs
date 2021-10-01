using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SwipePopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public enum AppearanceSide {
        Bottom,
        Top,
        Left,
        Right
    }

    [SerializeField]
    AnimationCurve _animationCurve;
    [SerializeField]
    AppearanceSide _appearanceSide;
    [SerializeField]
    RectTransform _swipeObjectTransform;
    [SerializeField]
    CanvasGroup _canvasGroup;

    [Range(0.001f, 20)]
    [SerializeField]
    float _time = 2;


    private CanvasScaler _canvasScaler;

    private Vector3 _initialPosition;
    private Vector2 _minMovedDistance;
    private bool _needMoving;

    private const float COEFIFICIENT = 0.4f;
    private const float COEFIFICIENT_HIDING = 1.21f;
    private const float MAX_DEVIATION = 60;
    private const float DEVIATION_COEFICIENT = 0.9f;

    public void UpdateMinMovedDistance() {
        _minMovedDistance = _swipeObjectTransform.rect.size * COEFIFICIENT;
    }

    public void OnDrag(PointerEventData eventData) {
        float deviation = 0;
        switch (_appearanceSide) {
            case AppearanceSide.Top:
                deviation = _swipeObjectTransform.offsetMax.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                Debug.Log("deviation " + deviation);
                if (deviation < -MAX_DEVIATION)
                    break;
                VerticalMoving(eventData, Mathf.Abs(Mathf.Clamp(deviation, -MAX_DEVIATION, 0)));
                break;
            case AppearanceSide.Bottom:
                deviation = _swipeObjectTransform.offsetMin.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                if (deviation > MAX_DEVIATION)
                    break;
                VerticalMoving(eventData, Mathf.Abs(Mathf.Clamp(deviation, 0, MAX_DEVIATION)));
                break;
            case AppearanceSide.Right:
                deviation = _swipeObjectTransform.offsetMax.x + eventData.delta.x / _canvasScaler.transform.localScale.x;
                if (deviation < -MAX_DEVIATION)
                    break;
                HorizontalMoving(eventData, Mathf.Abs(Mathf.Clamp(deviation, -MAX_DEVIATION, 0)));
                break;
            case AppearanceSide.Left:
                deviation = _swipeObjectTransform.offsetMin.x + eventData.delta.x / _canvasScaler.transform.localScale.x;
                if (deviation > MAX_DEVIATION)
                    break;
                HorizontalMoving(eventData, Mathf.Abs(Mathf.Clamp(deviation, 0, MAX_DEVIATION)));
                break;
        }

        Debug.Log("init position" + _swipeObjectTransform.localPosition + " " + _swipeObjectTransform.offsetMax.y);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        _initialPosition = _swipeObjectTransform.localPosition;
        UpdateMinMovedDistance();

        Debug.Log("init position" + _swipeObjectTransform.localPosition + " " + _swipeObjectTransform.offsetMax.y);
    }

    public void OnEndDrag(PointerEventData eventData) {
        float distanceMovedX = Mathf.Abs(_swipeObjectTransform.localPosition.x - _initialPosition.x);
        float distanceMovedY = Mathf.Abs(_swipeObjectTransform.localPosition.y - _initialPosition.y);

        _needMoving = distanceMovedX >= _minMovedDistance.x || distanceMovedY >= _minMovedDistance.y;

        if (!_needMoving) {
            _swipeObjectTransform.localPosition = _initialPosition;
        } else {
            StartCoroutine(MovedCard());
        }
    }

    private void Awake() {
        _canvasScaler = GetComponentInParent<CanvasScaler>();
    }

    private void VerticalMoving(PointerEventData eventData, float deviationMultiplier) {
        float delta = eventData.delta.y / _canvasScaler.transform.localScale.y * Mathf.Pow(DEVIATION_COEFICIENT, deviationMultiplier);
        float localPosition = _swipeObjectTransform.localPosition.y + delta;
        _swipeObjectTransform.localPosition = new Vector2(_swipeObjectTransform.localPosition.x, localPosition);
    }

    private void HorizontalMoving(PointerEventData eventData, float deviationMultiplier) {
        float delta = eventData.delta.x / _canvasScaler.transform.localScale.x * Mathf.Pow(DEVIATION_COEFICIENT, deviationMultiplier);
        float localPosition = _swipeObjectTransform.localPosition.x + delta;
        _swipeObjectTransform.localPosition = new Vector2(localPosition, _swipeObjectTransform.localPosition.y);
    }

    private IEnumerator MovedCard() {
        float time = 0;
        Vector3 startPosition = _swipeObjectTransform.localPosition;
        while (_canvasGroup.alpha > 0) {
            time += (Time.deltaTime / _time);



            _swipeObjectTransform.localPosition = Vector3.Slerp(startPosition,
                new Vector3(
                    _needMoving ? ((_appearanceSide == AppearanceSide.Left ? -_canvasScaler.referenceResolution.x : _canvasScaler.referenceResolution.x)) : _swipeObjectTransform.localPosition.x,
                    _needMoving ? ((_appearanceSide == AppearanceSide.Bottom ? -_canvasScaler.referenceResolution.y : _canvasScaler.referenceResolution.y)) : _swipeObjectTransform.localPosition.y,
                    0)
                    , Mathf.Clamp01(_animationCurve.Evaluate(time)));
            _canvasGroup.alpha = Mathf.SmoothStep(1, 0, time * COEFIFICIENT_HIDING);
            yield return null;
        }
    }
}
