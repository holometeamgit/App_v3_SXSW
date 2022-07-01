using System.Collections;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BeemMLARCamera : MonoBehaviour
{
    private Texture2D _texture;
    [SerializeField] private ARCameraManager _cameraManager;
    
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
        GetImageAsync();
    }

    private void GetImageAsync()
    {
        if (!_cameraManager.TryAcquireLatestCpuImage(out var image))
        {
            return;
        }
        
        StartCoroutine(ProcessImage(image));
        image.Dispose();
    }

    private IEnumerator ProcessImage(XRCpuImage image)
    {
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.RGB24,
            transformation = XRCpuImage.Transformation.MirrorY
        });

        while (!request.status.IsDone())
            yield return null;

        if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Request failed with status {0}", request.status);
            request.Dispose();
            yield break;
        }

        var rawData = request.GetData<byte>();

        if (_texture == null)
        {
            _texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false);
        }

        _texture.LoadRawTextureData(rawData);
        _texture.Apply();

        _image.texture = _texture;
        _beemMl.Invoke(_texture);
        
        //_image.material = _beemMl.transformMat;
        _outputView.texture = _beemMl.GetResultTexture();
        
        request.Dispose();
    }
}
