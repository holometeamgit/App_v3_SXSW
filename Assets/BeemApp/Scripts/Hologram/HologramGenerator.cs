using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Beem.Hologram {

    /// <summary>
    /// Hologram Creator
    /// </summary>
    public class HologramGenerator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [Header("Hologram Prefab")]
        [SerializeField]
        private GameObject _hologramPrefab;

        [Header("Touch Count")]
        [SerializeField]
        private int _touchCount = 1;

        private GameObject _spawnedObject;

        private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        private ARRaycastManager _raycastManager;

        private TouchCounter _touchCounter = new TouchCounter();

        private void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void OnEnable() {
#if UNITY_EDITOR
            CreateHologram(_hologramPrefab, _hologramPrefab.transform.position, _hologramPrefab.transform.rotation);
#endif
            HologramCallbacks.onSelectHologramPrefab += SetHologram;
        }

        private void OnDisable() {
            HologramCallbacks.onSelectHologramPrefab -= SetHologram;
        }

        private void SetHologram(GameObject hologram) {
            _hologramPrefab = hologram;
        }

        public void OnPointerDown(PointerEventData eventData) {

            _touchCounter.OnPointerDown(eventData);
            if (_touchCounter.TouchCount == _touchCount) {
                if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                    var hitPose = _hits[0].pose;
                    CreateHologram(_hologramPrefab, hitPose.position, hitPose.rotation);
                }
            }
        }

        private void CreateHologram(GameObject prefab, Vector3 position, Quaternion rotation) {
            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(prefab, position, rotation);
                HologramCallbacks.onCreateHologram?.Invoke(_spawnedObject);
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            _touchCounter.OnPointerUp(eventData);
        }
    }
}
