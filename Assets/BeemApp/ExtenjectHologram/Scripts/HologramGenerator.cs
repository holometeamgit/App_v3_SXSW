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

        private GameObject _spawnedObject;

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
#if UNITY_EDITOR
            CreateHologram(_hologramPrefab, _hologramPrefab.transform.position, _hologramPrefab.transform.rotation);
#endif
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
                Debug.Log("Holo");
                _spawnedObject = Instantiate(prefab, position, rotation);
                _signalBus.Fire(new CreateHologramSignal(_spawnedObject));
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Raycast");
            if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                var hitPose = _hits[0].pose;
                Debug.Log("CreateHologram");
                CreateHologram(_hologramPrefab, hitPose.position, hitPose.rotation);
            }
        }
    }
}
