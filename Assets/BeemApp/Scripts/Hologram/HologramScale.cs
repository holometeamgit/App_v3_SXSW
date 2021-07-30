using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Gologram Scale
    /// </summary>
    public class HologramScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {
        [Header("Hologram")]
        [SerializeField]
        private GameObject _hologram;
        [Header("Touch Count")]
        [SerializeField]
        private int _touchCount = 2;
        [Header("Zoom Speed")]
        [SerializeField]
        private float _zoomSpeed = 0.05f;
        [Header("Zoom Range")]
        [SerializeField]
        private Vector2 _zoomRange = new Vector2(0.35f, 1.95f);

        private float _startPerimeter;
        private float _endPerimeter;
        private TouchCounter _touchCounter = new TouchCounter();

        private void OnEnable() {
            HologramCallbacks.onCreateHologram += SetHologram;
        }

        private void OnDisable() {
            HologramCallbacks.onCreateHologram -= SetHologram;
        }

        private void SetHologram(GameObject hologram) {
            _hologram = hologram;
        }

        private Vector3 ClampDesiredScale(Vector3 desiredScale) {
            desiredScale = Vector3.Max(Vector3.one * _zoomRange.x, desiredScale);
            desiredScale = Vector3.Min(Vector3.one * _zoomRange.y, desiredScale);
            return desiredScale;
        }

        public void OnPointerDown(PointerEventData data) {
            _touchCounter.OnPointerDown(data);
        }

        public void OnPointerUp(PointerEventData data) {
            _touchCounter.OnPointerUp(data);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_touchCounter.TouchCount == _touchCount) {
                _endPerimeter = _touchCounter.TouchPerimeter;
                if (Mathf.Abs(_endPerimeter) > Mathf.Epsilon) {
                    float param = (_endPerimeter - _startPerimeter) / _endPerimeter;
                    var delta = Vector3.one * (param * _zoomSpeed);
                    if (_hologram != null) {
                        var desiredScale = _hologram.transform.localScale + delta;
                        desiredScale = ClampDesiredScale(desiredScale);
                        _hologram.transform.localScale = desiredScale;
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (_touchCounter.TouchCount == _touchCount) {
                _startPerimeter = _touchCounter.TouchPerimeter;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            _startPerimeter = 0;
            _endPerimeter = 0;
        }


    }
}