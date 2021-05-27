using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PointerPosition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    private GameObject _placedPrefab;

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject;

    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private ARRaycastManager _raycastManager;

    private TouchCounter touchCounter = new TouchCounter();

    private bool IsSpawned = false;

    private void Awake() {
        _raycastManager = FindObjectOfType<ARRaycastManager>();
    }
    public void OnPointerDown(PointerEventData eventData) {

        touchCounter.OnPointerDown(eventData);
        if (touchCounter.TouchCount == 1) {
            if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon) && !IsSpawned) {
                var hitPose = _hits[0].pose;
                IsSpawned = true;
                if (spawnedObject == null) {
                    spawnedObject = Instantiate(_placedPrefab, hitPose.position, hitPose.rotation);
                } else {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        touchCounter.OnPointerUp(eventData);
    }
}
