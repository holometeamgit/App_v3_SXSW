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
        VIDEO_LAUNCH, // user in video launch menu 
        SCANNING,     // scanning for tracking
        TAP_FIRST,    // tap to place after scanning
        LOADING,      // is video still loading
        PINCH,        // Pich to zoom or drag to replace
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
    
    // Remove it and add normal stuff
    [SerializeField] private FocusSquare _focusSquare;
    [SerializeField] private Transform _focusSquareV2Sprite;
    [SerializeField] private PnlViewingExperience _pnlViewingExperience;
    
    private void OnEnable()
    {
        SwitchToState(States.VIDEO_LAUNCH);
    }

    private void SwitchToState(States value)
    {
        switch (value)
        {
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
            case States.VIDEO_LAUNCH:
                TransformUpdate();
                if (VideoLoading)
                {
                    SwitchToState(States.SCANNING);
                }
                break;
            case States.SCANNING:
                TransformUpdate();
                
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
                
                break;
            case States.LOADING:
                //if (!VideoLoading) 
                {
                    SwitchToState(States.PINCH);
                }
                break;
            case States.PINCH:
                if (Input.touchCount > 1) 
                {
                    SwitchToState(States.HIDE);
                }
                break;
            case States.HIDE:
                TransformUpdate();
                HandleDistanceFade();

                if (SurfaceDetected())
                {
                    TapToPlace();
                }
                break;
        }
    }

    private bool SurfaceDetected() 
    {
        _hits = new List<ARRaycastHit>();
        m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), _hits, TrackableType.Planes);
        
        return _hits.Count > 0 && _arPlaneManager.trackables.count > 0;
    }

    private void HandleDistanceFade()
    {
        _focusSquareRenderer.color = new Color(1, 1, 1, GetAlphaBasedOnDistance());
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
        _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, _focusSquare.Quad.transform.position, Time.deltaTime * 20.0f);
        _focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, _focusSquare.Quad.transform.rotation, Time.deltaTime * 20.0f);
        _focusSquareV2Sprite.transform.localScale = _focusSquare.Quad.transform.localScale * 0.55f;
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
            OnPlaceDetected?.Invoke(_hits[0].pose.position);
            _hologramPlacedPosition = _hits[0].pose.position;
            
            return true;
        }

        return false;
    }
    
    private void TurnPlanes(bool value)
    {
        foreach (var plane in _arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }

        _arPlaneManager.enabled = value;
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
    
    // private void OnGUI()
    // {
    //     GUILayout.Space(400);
    //     GUILayout.Box(_currentState.ToString());
    //     GUILayout.Space(20);
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
    // }
}
