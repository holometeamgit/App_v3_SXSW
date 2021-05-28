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

    [SerializeField]
    private GameObject _spawnedObject;

    [SerializeField]
    private int fingers = 1;

    [SerializeField]
    private float minDistanceForNewPosition = 1f;

    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private ARRaycastManager _raycastManager;

    private TouchCounter touchCounter = new TouchCounter();

    private void Awake() {
        _raycastManager = FindObjectOfType<ARRaycastManager>();
    }
    public void OnPointerDown(PointerEventData eventData) {

        touchCounter.OnPointerDown(eventData);
        if (touchCounter.TouchCount == fingers) {
            if (_raycastManager.Raycast(eventData.pressPosition, _hits, TrackableType.PlaneWithinPolygon)) {
                var hitPose = _hits[0].pose;
                if (_spawnedObject == null) {
                    _spawnedObject = Instantiate(_placedPrefab, hitPose.position, hitPose.rotation);
                } else {
                    if (Vector3.Distance(_spawnedObject.transform.position, hitPose.position) > minDistanceForNewPosition) {
                        _spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        touchCounter.OnPointerUp(eventData);
    }
}
