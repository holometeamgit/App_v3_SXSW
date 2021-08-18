using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;

namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Gologram Position
    /// </summary>
    public class HologramPosition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

        [Header("Hologram")]
        [SerializeField]
        private GameObject _hologram;
        [Header("Touch Count")]
        [SerializeField]
        private int _touchCount = 1;

        [Header("Click Count")]
        [SerializeField]
        private int _clickCount = 2;

        [Header("Hologram Layer")]
        [SerializeField]
        private LayerMask _layerMask;

        private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
        private RaycastHit _hit = new RaycastHit();

        private ARRaycastManager _raycastManager;

        private TouchCounter _touchCounter = new TouchCounter();

        private bool isMoved = false;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<CreateHologramSignal>(SetHologram);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<CreateHologramSignal>(SetHologram);
        }

        private void SetHologram(CreateHologramSignal createHologramSignal) {
            _hologram = createHologramSignal.Hologram;
        }

        private void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        public void OnPointerDown(PointerEventData eventData) {
            _touchCounter.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            _touchCounter.OnPointerUp(eventData);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.clickCount == _clickCount) {
                ChangePosition(eventData);
            }
        }

        public void ChangePosition(PointerEventData eventData) {
            if (_touchCounter.TouchCount == _touchCount) {
                if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                    var hitPose = _hits[0].pose;
                    if (_hologram != null) {
                        _hologram.transform.position = hitPose.position;
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            Ray ray = Camera.main.ScreenPointToRay(eventData.pressPosition);
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _layerMask)) {
                isMoved = true;
            }
        }

        public void OnDrag(PointerEventData eventData) {
            if (isMoved) {
                ChangePosition(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            isMoved = false;
        }
    }
}