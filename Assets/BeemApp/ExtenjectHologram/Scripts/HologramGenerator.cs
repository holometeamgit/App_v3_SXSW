﻿using Beem.Extenject.Record;
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

        [SerializeField]
        private GameObject _targetPrefab;

        private GameObject _spawnedObject;

        public GameObject GetSpawnedObject() {
            return _spawnedObject;
        }

        private GameObject _spawnedTargetObject;

        private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        private ARRaycastManager _raycastManager;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void OnEnable() {
            _signalBus.Subscribe<SelectHologramSignal>(SetHologram);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<SelectHologramSignal>(SetHologram);
        }

        private void SetHologram(SelectHologramSignal selectHologramSignal) {
            _hologramPrefab = selectHologramSignal.Hologram;
        }

        private void CreateHologram(GameObject prefab, Vector3 position, Quaternion rotation) {
            if (_spawnedObject == null) {
                _spawnedTargetObject = Instantiate(_targetPrefab, position, rotation);
                _spawnedObject = Instantiate(prefab, position, rotation);
                _signalBus.Fire(new CreateHologramSignal(_spawnedObject));
            }
        }

        public void OnPointerClick(PointerEventData eventData) {

#if !UNITY_EDITOR
            if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                var hitPose = _hits[0].pose;
                CreateHologram(_hologramPrefab, hitPose.position, hitPose.rotation);
            }
#else
            CreateHologram(_hologramPrefab, _hologramPrefab.transform.position, _hologramPrefab.transform.rotation);
#endif
        }
    }
}
