using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SwipePopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public class ShowSwipePopUpSignal { public int id; }
    public class OnShowedSwipePopUpSignal { public int id; }
    public class HideSwipePopUpSignal { public int id; }
    public class OnHidedSwipePopUpSignal { public int id; }

    public enum AppearanceSide {
        Bottom,
        Top,
        Left,
        Right
    }

    [SerializeField] int id;
    [SerializeField]
    AnimationCurve _animationCurve;
    [SerializeField]
    AppearanceSide _appearanceSide;
    [SerializeField]
    RectTransform _swipedObjectTransform;
    [SerializeField]
    CanvasGroup _canvasGroup;

    [Range(0.001f, 20)]
    [SerializeField]
    float _time = 2;

    private CanvasScaler _canvasScaler;

    private Vector3 _initialPosition;
    private Vector2 _minMovedDistance;
    private bool _needMoving;
    private Vector3 _hidePosition;
    private Vector3 _showPosition;

    private const float COEFIFICIENT = 0.3f;
    //private const float COEFIFICIENT_HIDING = 1.21f;
    private const float MAX_DEVIATION = 60;
    private const float DEVIATION_COEFICIENT = 0.9f;

    public void OnDrag(PointerEventData eventData) {
        float deviation = 0;
        switch (_appearanceSide) {
            case AppearanceSide.Top:
                deviation = _swipedObjectTransform.offsetMax.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                Debug.Log("deviation " + deviation);
                if (deviation < -MAX_DEVIATION)
                    break;
                VerticalMoving(eventData, Mathf.Abs(Mathf.Clamp((int)deviation, (int)-MAX_DEVIATION, 0)));
                break;
            case AppearanceSide.Bottom:
                deviation = _swipedObjectTransform.offsetMin.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                if (deviation > MAX_DEVIATION)
                    break;
                VerticalMoving(eventData, Mathf.Abs(Mathf.Clamp((int)deviation, 0, (int)MAX_DEVIATION)));
                break;
            case AppearanceSide.Right:
                deviation = _swipedObjectTransform.offsetMax.x + eventData.delta.x / _canvasScaler.transform.localScale.x;
                if (deviation < -MAX_DEVIATION)
                    break;
                HorizontalMoving(eventData, Mathf.Abs(Mathf.Clamp((int)deviation, (int)-MAX_DEVIATION, 0)));
                break;
            case AppearanceSide.Left:
                deviation = _swipedObjectTransform.offsetMin.x + eventData.delta.x / _canvasScaler.transform.localScale.x;
                if (deviation > MAX_DEVIATION)
                    break;
                HorizontalMoving(eventData, Mathf.Abs(Mathf.Clamp((int)deviation, 0, (int)MAX_DEVIATION)));
                break;
        }

//        Debug.Log("init position" + _swipedObjectTransform.localPosition + " " + _swipedObjectTransform.offsetMax.y);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        _initialPosition = _swipedObjectTransform.localPosition;
        UpdateMinMovedDistance();
        CalculateTargetPoints();
        Debug.Log("targetPosition " + _hidePosition + " offsetMin " + _swipedObjectTransform.offsetMin.y + " rect.size " + (_swipedObjectTransform.rect.size.y - _canvasScaler.referenceResolution.y/2) + " " +_swipedObjectTransform.localPosition);
        //        Debug.Log("init position" + _swipedObjectTransform.localPosition + " " + _swipedObjectTransform.offsetMax.y);
    }

    public void OnEndDrag(PointerEventData eventData) {
        float distanceMovedX = Mathf.Abs(_swipedObjectTransform.localPosition.x - _initialPosition.x);
        float distanceMovedY = Mathf.Abs(_swipedObjectTransform.localPosition.y - _initialPosition.y);

        _needMoving = distanceMovedX >= _minMovedDistance.x || distanceMovedY >= _minMovedDistance.y;

        if (!_needMoving) {
            _swipedObjectTransform.localPosition = _initialPosition;
        } else {
            StartCoroutine(HideSwipedObject(_hidePosition, 0));
        }
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Awake() {
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        CalculateTargetPoints();
    }

    private void OnDisable() {
        _swipedObjectTransform.localPosition = _hidePosition;
        _canvasGroup.alpha = 0;
    }

    private void UpdateMinMovedDistance() {
        _minMovedDistance = _swipedObjectTransform.rect.size * COEFIFICIENT;
    }

    private void CalculateTargetPoints() {
        Debug.Log(_canvasScaler.referenceResolution.y);
        switch (_appearanceSide) {
            case AppearanceSide.Top:
                _hidePosition = new Vector3(_swipedObjectTransform.localPosition.x, _canvasScaler.referenceResolution.y, 0);
                _showPosition = new Vector3(_swipedObjectTransform.localPosition.x, _swipedObjectTransform.offsetMax.y, 0);
                break;
            case AppearanceSide.Bottom:
                _hidePosition = new Vector3(_swipedObjectTransform.localPosition.x, -_canvasScaler.referenceResolution.y, 0);
                _showPosition = new Vector3(_swipedObjectTransform.localPosition.x, _swipedObjectTransform.offsetMin.y, 0);
                break;
            case AppearanceSide.Right:
                _hidePosition = new Vector3(_canvasScaler.referenceResolution.x, _swipedObjectTransform.localPosition.y, 0);
                _showPosition = new Vector3(_swipedObjectTransform.offsetMax.x, _swipedObjectTransform.localPosition.y, 0);
                break;
            case AppearanceSide.Left:
                _hidePosition = new Vector3(-_canvasScaler.referenceResolution.x, _swipedObjectTransform.localPosition.y, 0);
                _showPosition = new Vector3(_swipedObjectTransform.offsetMin.x, _swipedObjectTransform.localPosition.y, 0);
                break;
        }
    }

    private void VerticalMoving(PointerEventData eventData, int deviationMultiplier) {
        float delta = eventData.delta.y / _canvasScaler.transform.localScale.y * Mathf.Pow(DEVIATION_COEFICIENT, deviationMultiplier);
        float localPosition = _swipedObjectTransform.localPosition.y + delta;
        _swipedObjectTransform.localPosition = new Vector2(_swipedObjectTransform.localPosition.x, localPosition);
    }

    private void HorizontalMoving(PointerEventData eventData, int deviationMultiplier) {
        float delta = eventData.delta.x / _canvasScaler.transform.localScale.x * Mathf.Pow(DEVIATION_COEFICIENT, deviationMultiplier);
        float localPosition = _swipedObjectTransform.localPosition.x + delta;
        _swipedObjectTransform.localPosition = new Vector2(localPosition, _swipedObjectTransform.localPosition.y);
    }

    private IEnumerator HideSwipedObject(Vector3 targetPosition, float targetAlpha) {
        float time = 0;
        Vector3 startPosition = _swipedObjectTransform.localPosition;
        while (time <= 1) {
            time += (Time.deltaTime / _time);

            _swipedObjectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, Mathf.Clamp01(_animationCurve.Evaluate(time)));
            _canvasGroup.alpha = Mathf.SmoothStep(1, 0, time);
            yield return null;
        }

        _swipedObjectTransform.localPosition = targetPosition;
        _canvasGroup.alpha = targetAlpha;
    }
}
