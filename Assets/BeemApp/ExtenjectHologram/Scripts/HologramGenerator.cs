using Beem.Extenject.Record;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Hologram Creator
    /// </summary>
    public class HologramGenerator : MonoBehaviour, IPointerClickHandler {

        [Header("Hologram Prefab")]
        [SerializeField]
        private GameObject _hologramPrefab;

        [Header("Move Touch Count")]
        [SerializeField]
        private int moveTouchCount = 1;
        [Header("Scale Touch Count")]
        [SerializeField]
        private int scaleTouchCount = 2;

        [Header("Zoom Speed")]
        [SerializeField]
        private float _zoomSpeed = 0.05f;
        [Header("Zoom Range")]
        [SerializeField]
        private Vector2 _zoomRange = new Vector2(0.35f, 1.95f);

        private TouchCounter _touchCounter = new TouchCounter();
        private GameObject _spawnedObject;
        private SignalBus _signalBus;
        private Transform _target;
        private float _startPerimeter;
        private float _endPerimeter;
        private bool isDrag;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<SelectHologramSignal>(SetHologram);
            _signalBus.Subscribe<CreateHologramTargetSignal>(SetTarget);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<SelectHologramSignal>(SetHologram);
            _signalBus.Unsubscribe<CreateHologramTargetSignal>(SetTarget);
        }

        private void SetHologram(SelectHologramSignal selectHologramSignal) {
            _hologramPrefab = selectHologramSignal.Hologram;
        }

        private void SetTarget(CreateHologramTargetSignal createHologramTargetSignal) {
            _target = createHologramTargetSignal.Target;
        }

        private void Hologram(Vector3 position, Quaternion rotation) {
            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(_hologramPrefab, position, rotation);
                _signalBus.Fire(new CreateHologramSignal(_spawnedObject));
            } else {
                if (_touchCounter.TouchCount == moveTouchCount) {
                    _spawnedObject.transform.position = position;
                    _spawnedObject.transform.rotation = rotation;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            Hologram(_target.position, _target.rotation);
        }

        public void OnPointerDown(PointerEventData eventData) {
            _touchCounter.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            _touchCounter.OnPointerUp(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_touchCounter.TouchCount == scaleTouchCount) {
                _endPerimeter = _touchCounter.TouchPerimeter;
                if (Mathf.Abs(_endPerimeter) > Mathf.Epsilon) {
                    float param = (_endPerimeter - _startPerimeter) / _endPerimeter;
                    var delta = Vector3.one * (param * _zoomSpeed);
                    if (_spawnedObject != null) {
                        var desiredScale = _spawnedObject.transform.localScale + delta;
                        desiredScale = ClampDesiredScale(desiredScale);
                        _spawnedObject.transform.localScale = desiredScale;
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (_touchCounter.TouchCount == scaleTouchCount) {
                _startPerimeter = _touchCounter.TouchPerimeter;
                isDrag = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            _startPerimeter = 0;
            _endPerimeter = 0;
            if (isDrag) {
                isDrag = false;
            }
        }

        private Vector3 ClampDesiredScale(Vector3 desiredScale) {
            desiredScale = Vector3.Max(Vector3.one * _zoomRange.x, desiredScale);
            desiredScale = Vector3.Min(Vector3.one * _zoomRange.y, desiredScale);
            return desiredScale;
        }

    }
}
