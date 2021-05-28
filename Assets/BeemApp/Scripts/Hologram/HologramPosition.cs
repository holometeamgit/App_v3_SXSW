using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Beem.Hologram {

    /// <summary>
    /// Gologram Position
    /// </summary>
    public class HologramPosition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [Header("Hologram")]
        [SerializeField]
        private GameObject _hologram;
        [Header("Touch Count")]
        [SerializeField]
        private int _touchCount = 1;
        [Header("Min Distance")]
        [SerializeField]
        private float _minDistance = 1f;

        private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        private ARRaycastManager _raycastManager;

        private TouchCounter _touchCounter = new TouchCounter();

        private void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void OnEnable() {
            HologramCallbacks.onCreateHologram += SetHologram;
        }

        private void OnDisable() {
            HologramCallbacks.onCreateHologram -= SetHologram;
        }

        private void SetHologram(GameObject hologram) {
            _hologram = hologram;
        }

        public void OnPointerDown(PointerEventData eventData) {
            _touchCounter.OnPointerDown(eventData);
            if (_touchCounter.TouchCount == _touchCount) {
                if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                    var hitPose = _hits[0].pose;
                    if (_hologram != null) {
                        if (Vector3.Distance(_hologram.transform.position, hitPose.position) > _minDistance) {
                            _hologram.transform.position = hitPose.position;
                        }
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            _touchCounter.OnPointerUp(eventData);
        }
    }
}