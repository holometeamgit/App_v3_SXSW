using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FocusSquareV2 : PlacementHandler
{
    private enum States
    {
        NOT_RUNNUNG,  // Use not at AR scene
        VIDEO_LAUNCH, // user in video launch menu 
        SCANNING,     // scanning for tracking
        TAP_FIRST,    // tap to place after scanning
        LOADING,      // is video still loading
        PINCH,        // Pich to zoom or drag to replace
        DELAY_AFTER_PINCH, // Short delay after pinch
        HIDE 
    }

    private States _currentState;
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

    // Remove it and add normal stuff
    [SerializeField] private FocusSquare _focusSquare;
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

    private void OnEnable()
    {
        SwitchToState(States.NOT_RUNNUNG);
    }

    private void SwitchToState(States value)
    {
        switch (value)
        {
            case States.NOT_RUNNUNG:
                TurnPlanes(false);
                _focusSquareV2Sprite.transform.position = new Vector3(100, 100, 100);
                _currentState = value;
                break;
            case States.VIDEO_LAUNCH:
                TurnPlanes(true);
                _currentState = value;
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
                TurnPlanes(false);
                // LoadignAnimation();
                _currentState = value;
                break;
            case States.PINCH:
                _focusSquareRenderer.color = new Color(1, 1, 1, 1.0f);
                _pnlViewingExperience.ShowPinchToZoomMessage();
                PinchToZoomAnimation();
                _currentState = value;
                break;
            case States.DELAY_AFTER_PINCH:
                _currentDelayAfterPinch = 0.0f;
                _currentState = value;
                break;
            case States.HIDE:
                TapToPlaceAnimation();
                _currentState = value;
                break;
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case States.NOT_RUNNUNG:
                if (IsOneOfButtonsCloseActive())
                {
                    SwitchToState(States.VIDEO_LAUNCH);
                }
                break;
            case States.VIDEO_LAUNCH:
                TransformUpdate();
                if (VideoLoading)
                {
                    SwitchToState(States.SCANNING);
                }
                break;
            case States.SCANNING:
                TransformUpdate();

                if (IsAllButtonsCloseNotActive())
                {
                    SwitchToState(States.NOT_RUNNUNG);
                }

                if (SurfaceDetected()) 
                {
                    SwitchToState(States.TAP_FIRST);
                }
                break;
            case States.TAP_FIRST:
                TransformUpdate();

                if (VideoQuadPlacing) 
                {
                    SwitchToState(States.LOADING);
                }

                if (!SurfaceDetected()) 
                {
                    SwitchToState(States.SCANNING);
                }
                else
                {
                    if (TapToPlace())
                    {
                        SwitchToState(States.LOADING); // TODO - check it
                    }
                }

                if (IsAllButtonsCloseNotActive())
                {
                    SwitchToState(States.NOT_RUNNUNG);
                }
                break;
            case States.LOADING:
                //if (!VideoLoading) 
                {
                    SwitchToState(States.PINCH);
                }
                break;
            case States.PINCH:
                // TODO add delay after pinch
                if (Input.touchCount > 1) 
                {
                    SwitchToState(States.DELAY_AFTER_PINCH);
                }

                if (IsAllButtonsCloseNotActive())
                {
                    SwitchToState(States.HIDE);
                }
                break;
            case States.DELAY_AFTER_PINCH:
                HandleDistanceFade();
                if (_currentDelayAfterPinch < _delayAfterPinch)
                {
                    _currentDelayAfterPinch += Time.deltaTime;
                    break;
                }
                _currentDelayAfterPinch = 0.0f;
                SwitchToState(States.HIDE);
                break;
            case States.HIDE:
                if (SurfaceDetected())
                {
                    TapToPlace();
                }

                TransformUpdate();
                HandleDistanceFade();

                break;
        }
    }

    private bool SurfaceDetected() 
    {
        _hits = new List<ARRaycastHit>();
        var ray = _arSessionOrigin.camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        m_RaycastManager.Raycast(ray, _hits, TrackableType.Planes);

        _raycastOrigin = ray.origin;
        _raycastDirection = ray.direction;

        return _hits.Count > 0 && _arPlaneManager.trackables.count > 0;
    }

    private void HandleDistanceFade()
    {
        if (_D <= _D_max)
        {
            _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, GetAlphaBasedOnDistance()), Time.deltaTime * 5.0f);
            return;
        }
       
        _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, 1 / (_D - _D_max + 1)), Time.deltaTime * 5.0f);
    }

    private void HandleMaxDistanceFade()
    {
        if (_D > _D_max)
        {
            _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, 1 / (_D - _D_max + 1)), Time.deltaTime * 5.0f);
        }
        else
        {
            _focusSquareRenderer.color = Color.Lerp(_focusSquareRenderer.color, new Color(1, 1, 1, 1), Time.deltaTime * 5.0f);
        }
    }

    private float GetAlphaBasedOnDistance()
    {
        return FadeBasedOnDistance(_hologramPlacedPosition, transparencyRangeHologram);
    }

    private float FadeBasedOnDistance(Vector3 fadeTargetPosition, float fullTransparencyRange)
    {
        return Mathf.Clamp(Vector3.Distance(GetZeroYVector(_focusSquare.Quad.transform.position), GetZeroYVector(fadeTargetPosition)) - fullTransparencyRange, 0, 1);
    }
    
    private void TransformUpdate() 
    {
        _H = -1;
        _D = -1;

        _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, _focusSquare.Quad.transform.position, Time.deltaTime * 25.0f);
        _focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, _focusSquare.Quad.transform.rotation, Time.deltaTime * 20.0f);
        _focusSquareV2Sprite.transform.localScale = _focusSquare.Quad.transform.localScale * 0.55f;

        if (_hits != null && _hits.Count > 0)
        {
            _D = _hits[0].distance;
            var plane = _arPlaneManager.GetPlane(_hits[0].trackableId);
            if (plane != null)
            {
                _H = plane.infinitePlane.GetDistanceToPoint(_arSessionOrigin.camera.transform.position);
            }
        }

        HandleMaxDistanceFade();

        if (_D < 0 || _H < 0)
        {
            return;
        }

        if (_D >= _D_max && _D_max > _H)
        {
            _x = _H * (1.0f - Mathf.Sqrt((_D_max * _D_max - _H * _H) / (_D * _D - _H * _H)));
            var ray = new Ray(_raycastOrigin - new Vector3(0, _x, 0), _raycastDirection);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            m_RaycastManager.Raycast(ray, hits, TrackableType.Planes);
            if (hits != null && hits.Count > 0)
            {
                _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, hits[0].pose.position, Time.deltaTime * 40.0f);
                return;
            }
        }
        else
        {
            _x = -1;
        }
    }
    
    private static Vector3 GetZeroYVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private bool TapToPlace()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended &&
            !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            //OnPlaceDetected?.Invoke(_hits[0].pose.position);
            //_hologramPlacedPosition = _hits[0].pose.position;

            OnPlaceDetected?.Invoke(_focusSquareV2Sprite.transform.position);
            _hologramPlacedPosition = _focusSquareV2Sprite.transform.position;

            return true;
        }

        return false;
    }
    
    private void TurnPlanes(bool value)
    {
        foreach (var plane in _arPlaneManager.trackables)
        {
            var arPlaneMeshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (arPlaneMeshVisualizer != null)
            {
                arPlaneMeshVisualizer.enabled = value;
            }
        }

        //_arPlaneManager.enabled = value;
    }

    private void TapToPlaceAnimation()
    {
        _focusSquareAnimator.ResetTrigger("PinchToZoom");
        _focusSquareAnimator.ResetTrigger("Scan");
        _focusSquareAnimator.SetTrigger("TapToPlace"); 
    }
    
    private void PinchToZoomAnimation()
    {
        _focusSquareAnimator.ResetTrigger("TapToPlace");
        _focusSquareAnimator.ResetTrigger("Scan");
        _focusSquareAnimator.SetTrigger("PinchToZoom");
    }
    
    private void LoadignAnimation()
    {
        _focusSquareAnimator.ResetTrigger("TapToPlace");
        _focusSquareAnimator.ResetTrigger("PinchToZoom");
        _focusSquareAnimator.SetTrigger("Scan");
    }

    private bool IsOneOfButtonsCloseActive()
    {
        return _btnCloseViewingExperience.activeInHierarchy ||
               _btnCloseStreamOverlay.activeInHierarchy;
    }

    private bool IsAllButtonsCloseNotActive()
    {
        return !_btnCloseViewingExperience.activeInHierarchy &&
               !_btnCloseStreamOverlay.activeInHierarchy;
    }

    //private void OnGUI()
    //{
        //GUILayout.Space(400);
        //GUILayout.Box("_D: " + _D.ToString());
        //GUILayout.Box("_H: " + _H.ToString());
        //GUILayout.Box("_x: " + _x.ToString());

        //GUILayout.Box("_currentState: " + _currentState.ToString());
        //_planePrefab.GetComponent<Renderer>().sharedMaterial.SetFloat("_AlphaFactor", GUILayout.HorizontalSlider(10 / Time.time, 0, 1));

        //GUILayout.Space(20);
        // GUILayout.Box(_btnClose.activeInHierarchy.ToString());
        //     
        //     if (GUILayout.Button("Video Loaded"))
        //     {
        //         VideoLoading = false;
        //     }
        //     
        //     GUILayout.Space(20);
        //     if (GUILayout.Button("Video Loading"))
        //     {
        //         VideoLoading = true;
        //     }
    //}
}
