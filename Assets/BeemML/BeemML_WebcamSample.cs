using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class BeemML_WebcamSample : MonoBehaviour
{
    [Tooltip("View with raw image")]
    [SerializeField]
    private WebCamTextureActivator webCamTexScript = null;

    [Tooltip("View with result mask")]
    [SerializeField]
    private RawImage outputView = null;
    
    [Tooltip("Options for required model")]
    [SerializeField]
    private BeemML.Options options = default;

    private BeemML beemMl;

    private void Start()
    {
        beemMl = new BeemML(options);

        webCamTexScript.OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        webCamTexScript.OnTextureUpdate.RemoveListener(OnTextureUpdate);

        beemMl?.Dispose();
    }
    
    /// <summary>
    /// Call our model's image processing and display the input and output view
    /// </summary>
    /// <param name="texture">input texture</param>
    private void OnTextureUpdate(Texture texture)
    {
        beemMl.Invoke(texture);
        
        outputView.material.SetTexture("_MaskTex", beemMl.GetResultTexture());
    }
}
