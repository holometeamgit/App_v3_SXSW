using Beem.Extenject.Record;
using System;
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
    public class HologramTargetGenerator : MonoBehaviour {

        [Header("Hologram Prefab")]
        [SerializeField]
        private Transform _hologramPrefab;

        private Transform _spawnedObject;

        private ARRaycastManager _raycastManager;
        private SignalBus _signalBus;
        private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Target(Vector3 position, Quaternion rotation) {
            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(_hologramPrefab, position, rotation);
                _signalBus.Fire(new CreateHologramTargetSignal(_spawnedObject));
            } else {
                _spawnedObject.position = position;
                _spawnedObject.rotation = rotation;
            }
        }

        private void Update() {
            MoveTarget(Target);
        }

        private bool SurfaceDetected() {
            _hits = new List<ARRaycastHit>();
            var ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            return _raycastManager.Raycast(ray, _hits, TrackableType.PlaneEstimated);
        }

        private void MoveTarget(Action<Vector3, Quaternion> onAction) {
#if !UNITY_EDITOR
            if (SurfaceDetected()) {
                var hitPose = _hits[0].pose;
                onAction?.Invoke(hitPose.position, hitPose.rotation);
            }
#else
            onAction?.Invoke(_hologramPrefab.transform.position, _hologramPrefab.transform.rotation);
#endif
        }
    }
}
