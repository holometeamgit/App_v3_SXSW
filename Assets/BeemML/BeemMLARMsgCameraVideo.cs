using System;
using System.Collections;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class BeemMLARMsgCameraVideo : MonoBehaviour
{
    [SerializeField] private ARMsgCameraPreview _arMsgCameraPreview;
    [SerializeField] private RawImage _image;
    [SerializeField] private RawImage _outputView;
    
    [SerializeField] private BeemML.Options _options;
    private BeemML _beemMl;
    
    private void Start()
    {
        _beemMl = new BeemML(_options);
    }
    
    private void OnDestroy()
    {
        _beemMl?.Dispose();
    }
    
    private void Update()
    {
        _beemMl.Invoke(_arMsgCameraPreview.cameraTexture);
        //_image.material = _beemMl.transformMat;
        _outputView.texture = _beemMl.GetResultTexture();
    }
}
