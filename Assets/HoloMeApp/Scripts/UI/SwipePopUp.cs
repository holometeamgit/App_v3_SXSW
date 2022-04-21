using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// controls for the movement of the popup
/// </summary>
public class SwipePopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public Action onStartShowing;
    public Action onShowed;
    public Action onStartHiding;
    public Action onHid;

    public enum AppearanceSide {
        Bottom,
        Top,
        Left,
        Right
    }

    [SerializeField]
    private int id;
    [SerializeField]
    private AnimationCurve _animationCurve;
    [SerializeField]
    private AppearanceSide _appearanceSide;
    [SerializeField]
    private RectTransform _swipedObjectTransform;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Range(0.001f, 20)]
    [SerializeField]
    private float _time = 2;

    private CanvasScaler _canvasScaler;

    private Vector3 _initialPosition;
    private Vector2 _minMovedDistance;
    private bool _needMoving;
    private Vector3 _hideOffsetPosition;
    private Vector3 _showOffsetPosition;
    private bool _isInit;

    private const float COEFFICIENT_MIN_MOVE_DISTANCE = 0.3f;
    private const float COEFFICIENT_HIDING = 1.21f;
    private const float MAX_DEVIATION = 60;
    private const float DEVIATION_COEFICIENT = 0.9f;

    public void OnDrag(PointerEventData eventData) {
        float deviation = 0;
        switch (_appearanceSide) {
            case AppearanceSide.Top:
                deviation = _swipedObjectTransform.offsetMax.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                if (deviation < -MAX_DEVIATION)
                    break;
                VerticalMoving(eventData, Mathf.Abs(Mathf.Clamp((int)deviation, (int)-MAX_DEVIATION, 0)));
                break;
            case AppearanceSide.Bottom:
                deviation = _swipedObjectTransform.offsetMin.y + eventData.delta.y / _canvasScaler.transform.localScale.y;
                Debug.LogError($"deviation {deviation} > MAX_DEVIATION {MAX_DEVIATION}");
                Debug.LogError($"cond {deviation > MAX_DEVIATION}");
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

    }

    public void OnBeginDrag(PointerEventData eventData) {
        _initialPosition = _swipedObjectTransform.localPosition;
        UpdateMinMovedDistance();
    }

    public void OnEndDrag(PointerEventData eventData) {
        float distanceMovedX = Mathf.Abs(_swipedObjectTransform.localPosition.x - _initialPosition.x);
        float distanceMovedY = Mathf.Abs(_swipedObjectTransform.localPosition.y - _initialPosition.y);

        _needMoving = distanceMovedX >= _minMovedDistance.x || distanceMovedY >= _minMovedDistance.y;

        if (_needMoving)
            Hide();
        else
            _swipedObjectTransform.offsetMin = _showOffsetPosition;
    }

    /// <summary>
    /// Show popup
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
        Init();
        onStartShowing?.Invoke();
        Move(isShow: true);
    }

    /// <summary>
    /// hide popup
    /// </summary>
    public void Hide() {
        onStartHiding?.Invoke();
        Move(isShow: false);
    }

    private void Awake() {
        Init();
    }

    private void OnDisable() {
        _swipedObjectTransform.offsetMin = _hideOffsetPosition;
        _canvasGroup.alpha = 0;
        StopAllCoroutines();
    }

    private void Init() {
        if (_isInit)
            return;
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        CalculateTargetPoints();
        _isInit = true;
    }

    private void Move(bool isShow) {
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)
            StartCoroutine(MovingObject(isShow));
    }

    private void UpdateMinMovedDistance() {
        _minMovedDistance = _swipedObjectTransform.rect.size * COEFFICIENT_MIN_MOVE_DISTANCE;
    }

    private void CalculateTargetPoints() {
        switch (_appearanceSide) {
            case AppearanceSide.Top:
                _hideOffsetPosition = new Vector2(_swipedObjectTransform.offsetMin.x, _canvasScaler.referenceResolution.y * COEFFICIENT_HIDING);
                _showOffsetPosition = new Vector2(_swipedObjectTransform.offsetMin.x, 0);
                break;
            case AppearanceSide.Bottom:
                _hideOffsetPosition = new Vector2(_swipedObjectTransform.offsetMin.x, -_canvasScaler.referenceResolution.y * COEFFICIENT_HIDING);
                _showOffsetPosition = new Vector2(_swipedObjectTransform.offsetMin.x, 0);
                break;
            case AppearanceSide.Right:
                _hideOffsetPosition = new Vector2(_canvasScaler.referenceResolution.x * COEFFICIENT_HIDING, _swipedObjectTransform.offsetMin.y);
                _showOffsetPosition = new Vector2(0, _swipedObjectTransform.offsetMin.y);
                break;
            case AppearanceSide.Left:
                _hideOffsetPosition = new Vector2(-_canvasScaler.referenceResolution.x * COEFFICIENT_HIDING, _swipedObjectTransform.offsetMin.y);
                _showOffsetPosition = new Vector2(0, _swipedObjectTransform.offsetMin.y);
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

    private IEnumerator MovingObject(bool isShow) {
        float time = 0;
        Vector2 startPosition = _swipedObjectTransform.offsetMin;
        Vector2 targetPosition = isShow ? _showOffsetPosition : _hideOffsetPosition;
        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = isShow ? 1 : 0;
        while (time <= 1) {
            time += (Time.deltaTime / _time);

            _swipedObjectTransform.offsetMin = Vector3.Lerp(startPosition, targetPosition, Mathf.Clamp01(_animationCurve.Evaluate(time)));
            _canvasGroup.alpha = Mathf.SmoothStep(startAlpha, targetAlpha, time);
            yield return null;
        }

        _swipedObjectTransform.offsetMin = targetPosition;
        _canvasGroup.alpha = targetAlpha;

        if (isShow) {
            onShowed?.Invoke();
        } else {
            onHid?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
