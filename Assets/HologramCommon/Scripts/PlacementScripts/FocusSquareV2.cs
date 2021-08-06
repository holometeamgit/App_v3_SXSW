using System;
using System.Collections;
using System.Collections.Generic;
using HoloMeSDK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;


public class FocusSquareV2 : PlacementHandler {
    private enum States {
        NOT_RUNNUNG,  // Use not at AR scene
        VIDEO_LAUNCH, // user in video launch menu 
        SCANNING,     // scanning for tracking
        TAP_FIRST,    // tap to place after scanning
        LOADING,      // is video still loading
        PINCH,        // Pich to zoom or drag to replace
        DELAY_AFTER_PINCH, // Short delay after pinch
        HIDE,         // Show/Hide tap to place after all
        DRAG_AND_DROP // Drag and drop state
    }

    private enum FocusAnimationStates {
        TAP,
        PINCH,
        LOADING
    }

    private States _currentState;
    private FocusAnimationStates _focusAnimationState;

    private List<ARRaycastHit> _hits;
    private Vector3 _hologramPlacedPosition = new Vector3(100, 100, 100);

    [SerializeField] private bool IsSurfaceDetected;
    [SerializeField] private bool VideoLoading;
    [SerializeField] private bool VideoQuadPlacing;
    [SerializeField] private bool VideoQuadMoving;

    [SerializeField] private SpriteRenderer _focusSquareRenderer;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private Animator _focusSquareAnimator;

    [SerializeField]
    [Range(0, 10)]
    [Tooltip("This is the unit distance before the square becomes transparent as it gets closer to the hologram")]
    private float transparencyRangeHologram = 1f;

    [SerializeField]
    private float _delayAfterPinch = 2.0f;
    private float _currentDelayAfterPinch = 0.0f;

    [SerializeField]
    private float _delayAfterLoading = 2.5f;
    private float _currentDelayAfterLoading = 0.0f;


    // Remove it and add normal stuff
    [SerializeField] private Transform _focusSquareV2Sprite;
    [SerializeField] private PnlViewingExperience _pnlViewingExperience;

    [SerializeField] private GameObject _btnCloseViewingExperience;
    [SerializeField] private GameObject _btnCloseStreamOverlay;

    [Space(20)]
    [SerializeField] private ARSessionOrigin _arSessionOrigin;

    private float _H;
    private float _D;
    [SerializeField] private float _D_max = 3.0f;
    private float _x;

    private Vector3 _raycastOrigin;
    private Vector3 _raycastDirection;

    [Space(20)]
    [SerializeField] private string _arPlanesLayerMaskName = "ARPlanes";
    private int _arPlanesLayerMask;

    public event Action OnPlacementUIHelperFinished;
    public HoloMe HoloMe {
        private get;
        set;
    }

    private bool _launchFirstTime = true;

    [SerializeField]
    private Transform _debugCapsule;

    private static List<ARRaycastHit> _hitsDrugAndDrop = new List<ARRaycastHit>();
    private Vector2 _touchPosition;

    [SerializeField]
    private float _maxDistanceOnSelection = 25.0f;

    // *** DEBUG ***
    private int _stopPlaneConstruction = -1;
    [SerializeField] private Toggle _stopPlaneConstructionCheckbox;
    // *** END DEBUG ***

    private void Awake() {
        _stopPlaneConstruction = PlayerPrefs.GetInt("_stopPlaneConstruction", -1);

        _stopPlaneConstructionCheckbox.isOn = _stopPlaneConstruction >= 0;
        _stopPlaneConstructionCheckbox.onValueChanged.AddListener(x => {
            _stopPlaneConstruction = x ? 1 : -1;
            PlayerPrefs.SetInt("_stopPlaneConstruction", _stopPlaneConstruction);
            Debug.Log("_stopPlaneConstruction PP: " + PlayerPrefs.GetInt("_stopPlaneConstruction", -1));
            Debug.Log("_stopPlaneConstruction : " + _stopPlaneConstruction);
        });
    }

    private void OnEnable() {
        _arPlanesLayerMask = LayerMask.NameToLayer(_arPlanesLayerMaskName);
        SwitchToState(States.NOT_RUNNUNG);
    }

    private void SwitchToState(States value) {
        switch (value) {
            case States.NOT_RUNNUNG:
                TurnPlanes(false);
                _focusSquareV2Sprite.transform.position = new Vector3(100, 100, 100);
                _hologramPlacedPosition = _focusSquareV2Sprite.transform.position;
                _currentState = value;
                break;
            case States.VIDEO_LAUNCH:
                if (_launchFirstTime) {
                    TurnPlanes(true);
                }

                _currentState = value;
                //_launchFirstTime = false;
                break;
            case States.SCANNING:
                TapToPlaceAnimation();
                _focusSquareRenderer.color = new Color(1, 1, 1, 0.0f);
                _pnlViewingExperience.RunTutorial();
                _currentState = value;
                break;
            case States.TAP_FIRST:
                _focusSquareRenderer.color = new Color(1, 1, 1, 1.0f);
                TapToPlaceAnimation();
                _pnlViewingExperience.ShowTapToPlaceMessage();
                _currentState = value;
                break;
            case States.LOADING:
                _focusSquareRenderer.color = new Color(1, 1, 1, 1.0f);
                _pnlViewingExperience.HideScanMessage();
                TurnPlanes(false);
                LoadignAnimation();
                _currentState = value;
                break;
            case States.PINCH:
                _focusSquareRenderer.color = new Color(1, 1, 1, 1.0f);
                _pnlViewingExperience.ShowPinchToZoomMessage();
                PinchToZoomAnimation();
                _currentState = value;
                break;
            case States.DELAY_AFTER_PINCH:
                _pnlViewingExperience.OnPlaced();
                _currentDelayAfterPinch = 0.0f;
                _currentState = value;
                break;
            case States.HIDE:
                TurnPlanes(false);
                if (_isHideStateReachFirstTime) {
                    _isHideStateReachFirstTime = false;
                    OnPlacementUIHelperFinished?.Invoke();
                }

                TapToPlaceAnimation();
                _currentState = value;
                break;
            case States.DRAG_AND_DROP:
                TurnPlanes(true);
                _currentState = value;
                break;
        }
    }

    private void Update() {
        switch (_currentState) {
            case States.NOT_RUNNUNG:
                if (IsOneOfButtonsCloseActive()) {
                    SwitchToState(States.VIDEO_LAUNCH);
                }
                break;
            case States.VIDEO_LAUNCH:
                TransformUpdate();

                if (IsAllButtonsCloseNotActive()) {
                    SwitchToState(States.NOT_RUNNUNG);
                    break;
                }

                if (!_launchFirstTime && VideoQuadPlacing) {
                    SwitchToState(States.LOADING);
                    break;
                }

                if (VideoLoading) {
                    SwitchToState(States.SCANNING);
                }
                break;
            case States.SCANNING:
                TransformUpdate();

                if (IsAllButtonsCloseNotActive()) {
                    SwitchToState(States.NOT_RUNNUNG);
                    break;
                }

                if (SurfaceDetected()) {
                    SwitchToState(States.TAP_FIRST);
                }
                break;
            case States.TAP_FIRST:
                TransformUpdate();
                HandleDistanceFade();

                if (VideoQuadPlacing) {
                    SwitchToState(States.LOADING);
                    break;
                }

                if (!SurfaceDetected()) {
                    SwitchToState(States.SCANNING);
                    break;
                } else {
                    if (TapToPlace()) {
                        SwitchToState(States.LOADING);
                        break;
                    }
                }

                if (IsAllButtonsCloseNotActive()) {
                    SwitchToState(States.NOT_RUNNUNG);
                }
                break;
            case States.LOADING:
                // if (!VideoLoading)
                if (IsAllButtonsCloseNotActive()) {
                    break;
                }

                if (SurfaceDetected()) {
                    TapToPlace();
                }

                TransformUpdate();

                if (HoloMe != null && HoloMe.IsPrepared) {
                    SwitchToState(_launchFirstTime ? States.PINCH : States.HIDE);
                    break;
                }

                //if (_uiThumbnailsController != null) TODO split for stream/non stream
                if (_currentDelayAfterLoading < _delayAfterLoading) {
                    _currentDelayAfterLoading += Time.deltaTime;
                    break;
                }

                _currentDelayAfterLoading = 0.0f;
                SwitchToState(_launchFirstTime ? States.PINCH : States.HIDE);
                break;
            case States.PINCH:
                if (IsAllButtonsCloseNotActive()) {
                    break;
                }

                if (Input.touchCount > 0) {
                    SwitchToState(States.DELAY_AFTER_PINCH);
                    break;
                }

                if (SurfaceDetected()) {
                    TapToPlace();
                    break;
                }

                TransformUpdate();
                break;
            case States.DELAY_AFTER_PINCH:

                if (IsAllButtonsCloseNotActive()) {
                    _currentDelayAfterPinch = _delayAfterPinch;
                    break;
                }

                HandleDistanceFade();

                if (SurfaceDetected()) {
                    TapToPlace();
                }

                TransformUpdate();

                if (_currentDelayAfterPinch < _delayAfterPinch) {
                    _currentDelayAfterPinch += Time.deltaTime;
                    break;
                }

                if (IsOneOfButtonsCloseActive()) {
                    _currentDelayAfterPinch = 0.0f;
                    _launchFirstTime = false;
                    SwitchToState(States.HIDE);
                }
                break;
            case States.HIDE:
                HideState();
                break;
            case States.DRAG_AND_DROP:
                if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                    _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, 0.0f), Time.deltaTime * 5.0f);
                    if (Input.touchCount == 1) {
                        var touch = Input.GetTouch(0);
                        switch (touch.phase) {
                            case TouchPhase.Moved:
                                _touchPosition = touch.position;
                                break;
                            case TouchPhase.Ended:
                                SwitchToState(States.HIDE);
                                break;
                        }
                    }

                    if (m_RaycastManager.Raycast(_touchPosition, _hitsDrugAndDrop, TrackableType.PlaneWithinPolygon)) {
                        var hitPose = _hitsDrugAndDrop[0].pose;
                        _debugCapsule.transform.position = hitPose.position;
                        _hologramPlacedPosition = hitPose.position;
                        OnPlaceDetected?.Invoke(_hologramPlacedPosition);
                    }
                }
                break;
        }
    }

    private bool _isHideStateReachFirstTime = true;

    private void HideState() {
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
            if (Input.touchCount == 1) {
                var touch = Input.GetTouch(0);
                switch (touch.phase) {
                    case TouchPhase.Began:
                        var ray = _arSessionOrigin.camera.ScreenPointToRay(touch.position);

                        if (Physics.Raycast(ray, out var hitObject, _maxDistanceOnSelection)) {
                            Debug.Log(hitObject.transform.name);
                            if (hitObject.transform.name.Contains("PlaceObject")) {
                                SwitchToState(States.DRAG_AND_DROP);
                            }
                        }
                        break;
                }
            }

            switch (_focusAnimationState) {
                case FocusAnimationStates.TAP: {
                        HandleDistanceFade();
                        if (!HoloMe.IsPrepared && _currentDelayAfterLoading <= _delayAfterLoading) {
                            LoadignAnimation();
                        }

                        break;
                    }
                case FocusAnimationStates.LOADING: {
                        if (HoloMe.IsPrepared) {
                            TapToPlaceAnimation();
                            break;
                        }

                        // TODO - fix it with Agora api
                        if (_currentDelayAfterLoading < _delayAfterLoading) {
                            _currentDelayAfterLoading += Time.deltaTime;
                        }

                        TapToPlaceAnimation();
                        break;
                    }
            }

            if (SurfaceDetected()) {
                TapToPlace();
            }

            TransformUpdate();

            if (IsAllButtonsCloseNotActive()) {
                _currentDelayAfterLoading = 0.0f;
            }
        }
    }

    private bool SurfaceDetected() {
        _hits = new List<ARRaycastHit>();
        var ray = _arSessionOrigin.camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        m_RaycastManager.Raycast(ray, _hits, TrackableType.PlaneEstimated);

        _raycastOrigin = ray.origin;
        _raycastDirection = ray.direction;

        return _hits.Count > 0 && _arPlaneManager.trackables.count > 0;
    }

    private float _angle;
    [SerializeField] private float _maxAngle = 10;
    private Vector3 _upWorldDirection;

    private void HandleDistanceFade() {
        var targetDir = _hologramPlacedPosition + _upWorldDirection * 1.02f - _arSessionOrigin.camera.transform.forward;
        _angle = Vector3.Angle(targetDir, _arSessionOrigin.camera.transform.position);

        // TODO - think about angle and what the fuck is going on here?
        if (_D <= _D_max) {
            _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, GetAlphaBasedOnDistance()), Time.deltaTime * 5.0f);
            return;
        }

        _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, 1 / (_D - _D_max + 1)), Time.deltaTime * 5.0f);
    }

    private float GetAlphaBasedOnDistance() {
        return FadeBasedOnDistance(_hologramPlacedPosition, transparencyRangeHologram);
    }

    private float FadeBasedOnDistance(Vector3 fadeTargetPosition, float fullTransparencyRange) {
        return Mathf.Clamp(Vector3.Distance(GetZeroYVector(_focusSquareV2Sprite.transform.position), GetZeroYVector(fadeTargetPosition)) - fullTransparencyRange, 0, 1);
    }

    private void TransformUpdate() {
        _H = -1;
        _D = -1;

        if (_hits != null && _hits.Count > 0) {
            _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, _hits[0].pose.position, Time.deltaTime * 25.0f);
            HandleOrientation();

            //_focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, _focusSquare.Quad.transform.position, Time.deltaTime * 25.0f);
            //_focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, _focusSquare.Quad.transform.rotation, Time.deltaTime * 20.0f);
            //_focusSquareV2Sprite.transform.localScale = _focusSquare.Quad.transform.localScale * 0.55f;

            _D = _hits[0].distance;
            var plane = _arPlaneManager.GetPlane(_hits[0].trackableId);
            if (plane != null) {
                _H = plane.infinitePlane.GetDistanceToPoint(_arSessionOrigin.camera.transform.position);
            }
        }

        if (_D < 0 || _H < 0) {
            return;
        }

        if (_D >= _D_max && _D_max > _H) {
            _x = _H * (1.0f - Mathf.Sqrt((_D_max * _D_max - _H * _H) / (_D * _D - _H * _H)));
            var ray = new Ray(_raycastOrigin - new Vector3(0, _x, 0), _raycastDirection);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            m_RaycastManager.Raycast(ray, hits, TrackableType.PlaneEstimated);
            if (hits != null && hits.Count > 0) {
                _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, hits[0].pose.position, Time.deltaTime * 40.0f);
                return;
            }
        } else {
            _x = -1;
        }
    }

    private static Vector3 GetZeroYVector(Vector3 vector) {
        return new Vector3(vector.x, 0, vector.z);
    }

    private bool TapToPlace() {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended &&
            !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
            //OnPlaceDetected?.Invoke(_hits[0].pose.position);
            //_hologramPlacedPosition = _hits[0].pose.position;

            OnPlaceDetected?.Invoke(_focusSquareV2Sprite.transform.position);
            _upWorldDirection = _focusSquareV2Sprite.transform.forward;
            _hologramPlacedPosition = _focusSquareV2Sprite.transform.position;
            _debugCapsule.position = _focusSquareV2Sprite.transform.position;

            TurnPlanes(false);

            return true;
        }

        return false;
    }

    private void TurnPlanes(bool value) {
        if (_stopPlaneConstruction > 0) {
            foreach (var plane in _arPlaneManager.trackables) {
                var arPlaneMeshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
                if (arPlaneMeshVisualizer != null) {
                    arPlaneMeshVisualizer.enabled = value;
                }
            }

            _arPlaneManager.enabled = value;
            return;
        }

        var oldMask = _arSessionOrigin.camera.cullingMask;
        var newMask = value ? oldMask | (1 << _arPlanesLayerMask) : oldMask & ~(1 << _arPlanesLayerMask);
        _arSessionOrigin.camera.cullingMask = newMask;


    }

    private void TapToPlaceAnimation() {
        _focusAnimationState = FocusAnimationStates.TAP;

        _focusSquareAnimator.ResetTrigger("PinchToZoom");
        _focusSquareAnimator.ResetTrigger("Scan");
        _focusSquareAnimator.SetTrigger("TapToPlace");
    }

    private void PinchToZoomAnimation() {
        _focusAnimationState = FocusAnimationStates.PINCH;

        _focusSquareAnimator.ResetTrigger("TapToPlace");
        _focusSquareAnimator.ResetTrigger("Scan");
        _focusSquareAnimator.SetTrigger("PinchToZoom");
    }

    private void LoadignAnimation() {
        _focusAnimationState = FocusAnimationStates.LOADING;

        _focusSquareAnimator.ResetTrigger("TapToPlace");
        _focusSquareAnimator.ResetTrigger("PinchToZoom");
        _focusSquareAnimator.SetTrigger("Scan");
    }

    private bool IsOneOfButtonsCloseActive() {
        return _btnCloseViewingExperience.activeInHierarchy ||
               _btnCloseStreamOverlay.activeInHierarchy;
    }

    private bool IsAllButtonsCloseNotActive() {
        return !_btnCloseViewingExperience.activeInHierarchy &&
               !_btnCloseStreamOverlay.activeInHierarchy;
    }

    private void HandleOrientation() {
        var cameraForwardBearing = new Vector3(_arSessionOrigin.camera.transform.forward.x, -90, _arSessionOrigin.camera.transform.forward.z).normalized;
        _focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, Quaternion.LookRotation(cameraForwardBearing), Time.deltaTime * 15f);
    }

    private void OnGUI() {
        //     GUILayout.Space(400);
        //      GUILayout.Box("_angle: " + _angle);
        //      GUILayout.Box("_camera angles: " + _arSessionOrigin.camera.transform.eulerAngles.ToString());
        // //     // GUILayout.Box("_x: " + _x.ToString());
        //
        //     GUILayout.Box("_currentState: " + _currentState.ToString());
        //     //_planePrefab.GetComponent<Renderer>().sharedMaterial.SetFloat("_AlphaFactor", GUILayout.HorizontalSlider(10 / Time.time, 0, 1));
        //
        //     // GUILayout.Space(20);
        //     //GUILayout.Box(_btnClose.activeInHierarchy.ToString());
        //          
        //          // if (GUILayout.Button("Video Loaded"))
        //          // {
        //          //     VideoLoading = false;
        //          // }
        //          //
        //     GUILayout.Space(20);
        // if (GUILayout.Button("Video Loading"))
        // {
        //     VideoLoading = true;
        // }
        // if (GUILayout.Button("On"))
        // {
        //     TurnPlanes(true);
        // }
        //
        // GUILayout.Space(20);
        // if (GUILayout.Button("Off"))
        // {
        //     TurnPlanes(false);
        // }
    }
}
