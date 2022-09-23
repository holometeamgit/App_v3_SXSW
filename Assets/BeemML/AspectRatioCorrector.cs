using UnityEngine;
using UnityEngine.UI;

public class AspectRatioCorrector : MonoBehaviour
{
    [SerializeField]
    private AspectRatioFitter aspectRatioFitter;
        
    private void Start() {
        aspectRatioFitter.aspectRatio = (float)Screen.width / Screen.height;
    }
}
