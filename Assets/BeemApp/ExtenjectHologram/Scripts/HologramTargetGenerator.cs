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

        private SignalBus _signalBus;
        private bool _arActive;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void ARSessionActivate(ARSessionActivateSignal signal) {
            if (signal.Active) {
#if UNITY_EDITOR
                _signalBus.Fire(new ARPlanesDetectedSignal(true));
                ActivateTargetAsync();
#endif
            }
        }

        private void OnEnable() {
            _signalBus.Subscribe<ARSessionActivateSignal>(ARSessionActivate);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<ARSessionActivateSignal>(ARSessionActivate);
        }


        private async void ActivateTargetAsync() {
            await Task.Yield();
            _signalBus.Fire(new TargetPlacementSignal(_hologramPrefab.transform));
        }

#if !UNITY_EDITOR

        private ARRaycastManager _arRaycastManager;
        private Pose _placementPose;
        private bool _placementPoseIsValid = false;

         private void Awake() {
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        }

        private void Update() {
            if(_arActive){
                UpdatePlacementPose();
                UpdatePlacementIndicator();
            }
        }

        private void UpdatePlacementIndicator() {
            if (_placementPoseIsValid) {
                ActivateTargetAsync();
                UpdateTransform();
            }
        }

        private void UpdateTransform() {
            if (_hologramPrefab != null) {
                _hologramPrefab.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            }
        }

        private void UpdatePlacementPose() {

            var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            _arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);
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
