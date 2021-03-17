using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FocusSquare : PlacementHandler
{
    [SerializeField]
    Texture2D focusSquareTexture;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("This is the unit distance of how close the user can get to the camera before the square goes transparent")]
    private float transparencyRangeUser = 0.15f;

    [SerializeField]
    [Range(0, 10)]
    [Tooltip("This is the unit distance before the square becomes transparent as it gets closer to the hologram")]
    private float transparencyRangeHologram = 1f;

    [SerializeField]
    [Tooltip("Check this to have the focus square smoothly follow the camera")]
    private bool smoothFollow = true;

    [SerializeField]
    [Range(2, 10)]
    [Tooltip("This determines the speed of the smooth movement")]
    float smoothFollowSpeed = 5;

    [Tooltip("Use this to apply custom shaders, Unlit/Transparent recommended")]
    [SerializeField]
    Shader shader;

    [SerializeField]
    Camera arCamera;

    Material quadMat;
    GameObject quad;

    public GameObject Quad => quad;
    
    //[SerializeField]
    //bool stayOnAfterPlace;

    [Tooltip("Use this to force the logo to always display in the correct orientation")]
    [SerializeField]
    bool keepOrientation = true;

    Transform lookTarget;

    Vector3 hologramPlacedPosition = new Vector3(100, 100, 100); //Start away from the user

    // [SerializeField]
    // UnityEvent OnSurfaceFound;
    
    // [SerializeField]
    // UnityEvent OnSurfaceLost;

    [SerializeField]
    UnityEvent OnPlaced;

    [SerializeField]
    UnityEvent OnPinch;

    bool inAnimation;

    bool surfaceDetected;
    public bool SurfaceDetected
    {
        get => surfaceDetected;
        set
        {
            bool valueChanged = surfaceDetected != value;
            surfaceDetected = value;

            if (surfaceDetected)
            {
                if (valueChanged)
                {
                    //Debug.Log("VALUE CHANGED SURFACE FOUND");
                    //StopAllCoroutines();
                    //StartCoroutine(FadeHide(false));
                    SwitchToState(States.TAP);
                    
                }
            }
            else
            {
                if (valueChanged)
                {
                    //Debug.Log("VALUE CHANGED NO SURFACE");
                    //StopAllCoroutines();
                    //StartCoroutine(FadeHide(true));
                    //SwitchToState(States.SCAN);

                    // if (_currentState != States.PINCH) {
                    //     OnSurfaceLost?.Invoke();
                    // }
                }
            }
        }
    }

    public bool StartScanning { get; set; }

    [Space(20)]
    // [SerializeField] private Transform _focusSquareV2Sprite;
    // [SerializeField] private Animator _focusSquareAnimator;
    [SerializeField] private Text _debugText;
    // [SerializeField] private SpriteRenderer _focusSquareRenderer;

    private enum States 
    {
        SCAN,
        TAP,
        PINCH,
        HIDE,
    }

    private States _currentState = States.HIDE;
    private float _currentDelay;
    private float _delayBeforeRescan = 3.0f;

    [SerializeField] private Camera _camera;

    private bool _wasOnePinch;
    private bool _delayAfterPinch;

    // private ARPlaneManager _arPlaneManager;

    [SerializeField] private ARSessionOrigin _arSessionOrigin;

    public void Awake()
    {
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "FocusSquare";
        UpdateUnitSize(0.25f);
        quad.transform.Rotate(new Vector3(90, 0, 0));
        Destroy(quad.GetComponent<MeshCollider>());

        quadMat = quad.GetComponent<Renderer>().material;
        quad.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/FocusSquare");
        SetFocusSquareTexture(focusSquareTexture);

        StartCoroutine(FadeHide(true));
        
        SwitchToState(States.SCAN);

        // _arPlaneManager = _arSessionOrigin.GetComponent<ARPlaneManager>(); 
        //OnSurfaceLost?.Invoke();
    }

    private void SetFocusSquareTexture(Texture2D texture)
    {
        if (texture != null)
        {
            quadMat.SetTexture("_MainTex", texture);
        }
        else if (Application.isEditor)
        {
            Debug.LogError("Passed in focus square texture was null");
        }
    }

    IEnumerator FadeHide(bool hide)
    {
        inAnimation = true;
        var quadMatCurrentColor = quadMat.GetColor("_Color");
        while (hide ? quadMatCurrentColor.a > 0 : quadMatCurrentColor.a < GetAlphaBasedOnDistance())
        {
            quadMat.SetColor("_Color", new Color(quadMatCurrentColor.r, quadMatCurrentColor.g, quadMatCurrentColor.b, quadMatCurrentColor.a -= hide ? 0.05f : -0.05f));
            yield return new WaitForSeconds(0.015f);
        }
        inAnimation = false;
    }

    public void UpdateUnitSize(float unitSize)
    {
        quad.transform.localScale = new Vector3(unitSize, unitSize, quad.transform.localScale.z);
    }

    private float GetAlphaBasedOnDistance()
    {
        //var fadeValueUser = FadeBasedOnDistance(arCamera.transform.position, transparencyRangeUser);
        //var fadeValueHologram = FadeBasedOnDistance(hologramPlacedPosition, transparencyRangeHologram);
        //return Mathf.Min(fadeValueUser, fadeValueHologram);
        return FadeBasedOnDistance(hologramPlacedPosition, transparencyRangeHologram);
    }

    public void Hide()
    {
        quad.SetActive(false);
    }

    private void HandleOrientation()
    {
        if (keepOrientation)
        {
            var cameraForwardBearing = new Vector3(arCamera.transform.forward.x, -90, arCamera.transform.forward.z).normalized;
            quad.transform.rotation = Quaternion.Slerp(quad.transform.rotation, Quaternion.LookRotation(cameraForwardBearing), Time.deltaTime * 15f);
        }
    }

    private void HandleDistanceFade()
    {
        if (SurfaceDetected && !inAnimation)
        {
            quadMat.color = new Color(1, 1, 1, GetAlphaBasedOnDistance());

            if (_currentState != States.HIDE && _currentState != States.PINCH) {
                // *** _focusSquareRenderer.color = new Color(1, 1, 1, GetAlphaBasedOnDistance());
            }
            else 
            {
                // *** _focusSquareRenderer.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private float FadeBasedOnDistance(Vector3 fadeTargetPosition, float fullTransparencyRange)
    {
        return Mathf.Clamp(Vector3.Distance(GetZeroYVector(quad.transform.position), GetZeroYVector(fadeTargetPosition)) - fullTransparencyRange, 0, 1);
    }

    private static Vector3 GetZeroYVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private void FollowCamera(Vector3 position)
    {
        if (smoothFollow)
        {
            quad.transform.position = Vector3.Lerp(quad.transform.position, new Vector3(position.x, position.y + 0.1f, position.z), Time.deltaTime * smoothFollowSpeed);
        }
        else
        {
            quad.transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
        }
    }

    private void Update()
    {
        if (!StartScanning)
            return;

        var hits = new List<ARRaycastHit>();
        m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        SurfaceDetected = hits.Count > 0;

        if (SurfaceDetected)
        {
            if (!_delayAfterPinch)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                //||
                //(forcePlace && Vector3.Distance(arCamera.transform.position, hits[0].pose.position) >= 1))
                {
                    //forcePlace = false;
                    
                    // OnPlaced?.Invoke();
                    // OnPlaceDetected?.Invoke(hits[0].pose.position);
                    
                    hologramPlacedPosition = quad.transform.position;
                    SwitchToState(States.PINCH);
                    
                    // *** _focusSquareV2Sprite.transform.position = quad.transform.position;
                    // *** _focusSquareV2Sprite.transform.rotation = quad.transform.rotation;
                    // ***  _focusSquareV2Sprite.transform.localScale = quad.transform.localScale * 0.55f;

                    //print($"Positions camera = {arCamera.transform.position} Position for place {hits[0].pose.position}");
                }
            }
            
            FollowCamera(hits[0].pose.position);
        }
        _delayAfterPinch = false;
        
        if (_currentState != States.SCAN)
        {
            //m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneEstimated);
            if (hits.Count > 0) 
            {
                _currentDelay = 0.0f;
                SurfaceDetected = true;
            }
            else
            {
                _currentDelay += Time.deltaTime;
            }

            if (_currentDelay > _delayBeforeRescan) 
            {
                _currentDelay = 0.0f;
                
                SurfaceDetected = false;
                SwitchToState(States.SCAN);
            }
        }
        else
        {
            _currentDelay = 0.0f;
        }

        switch (_currentState) 
        {
            case States.SCAN:
                //FocusSquareV2UpdateCenter();
                //m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinInfinity);
                //FocusSquareV2Update();
            break;
            case States.TAP: 
                FocusSquareV2Update();   
            break;
            case States.PINCH:
                if (Input.touchCount > 1)
                {             
                    SwitchToState(States.HIDE);
                    _delayAfterPinch = true;
                }
            break;
            case States.HIDE:
            break;
        }

        //FocusSquareV2Update();

        HandleOrientation();
        HandleDistanceFade();
    }

    private void FocusSquareV2Update() 
    {
        // *** _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, quad.transform.position, Time.deltaTime * 20.0f);
        // *** _focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, quad.transform.rotation, Time.deltaTime * 20.0f);
        // *** _focusSquareV2Sprite.transform.localScale = quad.transform.localScale * 0.55f;
    }

    /*** private void FocusSquareV2UpdateCenter()
    {
        Vector3 center = new Vector3(Screen.width/2, Screen.height/2 - 0.1f, 2.0f);
        _focusSquareV2Sprite.transform.position = Vector3.Lerp(_focusSquareV2Sprite.transform.position, _camera.ScreenToWorldPoint(center), Time.deltaTime * 20.0f);

        Vector3 vecToCamera = _focusSquareV2Sprite.transform.position - _camera.transform.position;
        Vector3 vecOrthogonal = Vector3.Cross(vecToCamera, Vector3.up);
        Vector3 vecForward = Vector3.Cross(vecOrthogonal, Vector3.up);

        _focusSquareV2Sprite.transform.rotation = Quaternion.Slerp(_focusSquareV2Sprite.transform.rotation, Quaternion.LookRotation(vecForward, Vector3.down), Time.deltaTime * 20.0f);
        
        var scale = _focusSquareV2Sprite.transform.localScale;
        _focusSquareV2Sprite.transform.localScale = new Vector3(-0.1f, -0.1f, -0.2f);
        //_focusSquareV2Sprite.transform.localEulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    } ***/

    // TODO fix OnPinch?.Invoke(); 
    private void SwitchToState(States value) 
    {
        // TODO
        // 2 Thinking about scan position and rescan
        switch (value) 
        {
            case States.SCAN:
                // if (_currentState == States.PINCH) 
                // {
                //     break;
                // }
                
                // **** OnSurfaceLost?.Invoke(); ****
                
                _currentState = value;
                _debugText.text = _currentState.ToString();
                /*** _focusSquareAnimator.ResetTrigger("TapToPlace");
                _focusSquareAnimator.ResetTrigger("PinchToZoom");
                _focusSquareAnimator.SetTrigger("Scan"); ***/
                StopAllCoroutines();
                StartCoroutine(FadeHideV2(false));
            break;
            case States.TAP:
                if (_currentState == States.PINCH) 
                {
                    break;
                }

                // **** OnSurfaceFound?.Invoke(); ****
                
                _currentState = value;
                _debugText.text = _currentState.ToString();
                /*** _focusSquareAnimator.ResetTrigger("PinchToZoom");
                _focusSquareAnimator.ResetTrigger("Scan");
                _focusSquareAnimator.SetTrigger("TapToPlace"); ***/    
            break;
            case States.PINCH:
                if (_wasOnePinch) 
                {
                    SwitchToState(States.HIDE);
                    break;
                }
                
                // TurnOffPlanes();

                _currentState = value;
                _debugText.text = _currentState.ToString();
                /*** _focusSquareAnimator.ResetTrigger("TapToPlace");
                _focusSquareAnimator.ResetTrigger("Scan");
                _focusSquareAnimator.SetTrigger("PinchToZoom"); ***/
            break;
            case States.HIDE:
                OnPinch?.Invoke();
                _wasOnePinch = true;
                _currentState = value;
                _debugText.text = _currentState.ToString();
                StopAllCoroutines();
                StartCoroutine(FadeHideV2(true));
            break;
        }
    }

    IEnumerator FadeHideV2(bool hide)
    {
        inAnimation = true;
        // *** var quadMatCurrentColor = _focusSquareRenderer.material.GetColor("_Color");
        // *** while (hide ? quadMatCurrentColor.a > 0 : quadMatCurrentColor.a < GetAlphaBasedOnDistance())
        {
            // *** _focusSquareRenderer.material.SetColor("_Color", new Color(quadMatCurrentColor.r, quadMatCurrentColor.g, quadMatCurrentColor.b, quadMatCurrentColor.a -= hide ? 0.05f : -0.05f));
            yield return new WaitForSeconds(0.015f);
        }
        inAnimation = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // private void TurnOffPlanes()
    // {
    //     foreach (var plane in _arPlaneManager.trackables)
    //     {
    //         plane.gameObject.SetActive(false);
    //     }
    //
    //     _arPlaneManager.enabled = false;
    // }
}
