using Beem.Extenject.Record;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [SerializeField]
        private GameObject _hologramPrefab;

        private GameObject _spawnedObject;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Fire(new ARSignal(true));
#if UNITY_EDITOR
            _signalBus.Fire(new ARPlanesDetectedSignal(true));
            ActivateTargetAsync();
#endif
        }

        private void OnDisable() {
            _signalBus.Fire(new ARSignal(false));
            DeactivateTarget();
        }


        private async void ActivateTargetAsync() {

            await Task.Yield();

            if (_spawnedObject == null) {
                _spawnedObject = Instantiate(_hologramPrefab);
                _signalBus.Fire(new TargetPlacementSignal(_spawnedObject.transform));
            }
            _spawnedObject.SetActive(true);
        }

        private void DeactivateTarget() {
            if (_spawnedObject != null) {
                _spawnedObject.SetActive(false);
            }
        }

#if !UNITY_EDITOR

        private ARRaycastManager _arRaycastManager;
        private Pose _placementPose;
        private bool _placementPoseIsValid = false;

         private void Awake() {
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Update() {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }

        private void UpdatePlacementIndicator() {
            if (_placementPoseIsValid) {
                ActivateTargetAsync();
                UpdateTransform();
            } else {
                DeactivateTarget();
            }
        }

        private void UpdateTransform() {
            if (_spawnedObject != null) {
                _spawnedObject.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            }
        }

        private void UpdatePlacementPose() {

            var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            _arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
            _placementPoseIsValid = hits.Count > 0;
            if (_placementPoseIsValid) {
                _placementPose = hits[0].pose;

                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
            
            _signalBus.Fire(new ARPlanesDetectedSignal(_placementPoseIsValid));
        }
#endif
    }
}
