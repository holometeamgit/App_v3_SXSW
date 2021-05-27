using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField]
    private Text text;
    [SerializeField]
    private GameObject _spawnedObject;
    [Space]
    [SerializeField]
    private float zoomSpeed = 0.05f;
    [Space]
    [SerializeField]
    private float minZoom = 0.35f;
    [SerializeField]
    private float maxZoom = 1.95f;

    private float startPerimeter;
    private float endPerimeter;
    private TouchCounter touchCounter = new TouchCounter();

    private void Update() {
        text.text = touchCounter.TouchDifference.ToString();
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale) {
        desiredScale = Vector3.Max(Vector3.one * minZoom, desiredScale);
        desiredScale = Vector3.Min(Vector3.one * maxZoom, desiredScale);
        return desiredScale;
    }

    public void OnPointerDown(PointerEventData data) {
        touchCounter.OnPointerDown(data);
    }

    public void OnPointerUp(PointerEventData data) {
        touchCounter.OnPointerDown(data);
    }

    public void OnDrag(PointerEventData eventData) {
        if (touchCounter.TouchCount == 2) {
            endPerimeter = touchCounter.TouchPerimeter;
            if (Mathf.Abs(endPerimeter) > Mathf.Epsilon) {
                float param = (endPerimeter - startPerimeter) / endPerimeter;
                //text.text = param.ToString();
                var delta = Vector3.one * (param * zoomSpeed);
                var desiredScale = _spawnedObject.transform.localScale + delta;
                desiredScale = ClampDesiredScale(desiredScale);
                _spawnedObject.transform.localScale = desiredScale;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (touchCounter.TouchCount == 2) {
            startPerimeter = touchCounter.TouchPerimeter;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        startPerimeter = 0;
        endPerimeter = 0;
    }


}