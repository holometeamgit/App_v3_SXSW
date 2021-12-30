﻿using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Hologram Controller
    /// </summary>
    public class HologramController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler {

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
            _signalBus.Subscribe<TargetPlacementSignal>(SetTarget);

        }

        private void OnDisable() {
            _signalBus.Unsubscribe<SelectHologramSignal>(SetHologram);
            _signalBus.Unsubscribe<TargetPlacementSignal>(SetTarget);
            DeactivateHologram();
        }

        private void SetHologram(SelectHologramSignal selectHologramSignal) {
            _hologramPrefab = selectHologramSignal.Hologram;
        }

        private void SetTarget(TargetPlacementSignal createHologramTargetSignal) {
            _target = createHologramTargetSignal.Target;
        }

        private void ActivateHologram(Vector3 position, Quaternion rotation) {
            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(_hologramPrefab);
                _signalBus.Fire(new HologramPlacementSignal(_spawnedObject));
            }

            _spawnedObject.transform.SetPositionAndRotation(position, rotation);

        }

        private void DeactivateHologram() {
            if (_spawnedObject != null) {
                _signalBus.Fire(new HologramPlacementSignal());
                _signalBus.Fire(new ARPinchSignal(false));
                _target = null;
                Destroy(_spawnedObject);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            _touchCounter.OnPointerDown(eventData);
            if (_touchCounter.TouchCount == moveTouchCount) {
                if (_target != null && _spawnedObject == null) {
                    ActivateHologram(_target.position, _target.rotation);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            _touchCounter.OnPointerUp(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            if (_touchCounter.TouchCount == scaleTouchCount) {
                if (_spawnedObject != null) {
                    _endPerimeter = _touchCounter.TouchPerimeter;
                    if (Mathf.Abs(_endPerimeter) > Mathf.Epsilon) {
                        ChangeScale((_endPerimeter - _startPerimeter) / _endPerimeter);
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
                _signalBus.Fire(new ARPinchSignal(true));
            }
        }

        public void OnScroll(PointerEventData eventData) {
#if UNITY_EDITOR
            if (_spawnedObject != null) {
                ChangeScale(eventData.scrollDelta.y);
                if (Mathf.Abs(eventData.scrollDelta.y) > 0) {
                    _signalBus.Fire(new ARPinchSignal(true));
                }
            }
#endif
        }

        private void ChangeScale(float param) {
            var delta = Vector3.one * (param * _zoomSpeed);
            var desiredScale = _spawnedObject.transform.localScale + delta;
            desiredScale = ClampDesiredScale(desiredScale);
            _spawnedObject.transform.localScale = desiredScale;
        }

        private Vector3 ClampDesiredScale(Vector3 desiredScale) {
            desiredScale = Vector3.Max(Vector3.one * _zoomRange.x, desiredScale);
            desiredScale = Vector3.Min(Vector3.one * _zoomRange.y, desiredScale);
            return desiredScale;
        }
    }
}
