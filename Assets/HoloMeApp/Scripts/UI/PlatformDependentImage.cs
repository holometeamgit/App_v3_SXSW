using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlatformDependentImage : MonoBehaviour
{
    [SerializeField]
    private Sprite _appleImage;
    [SerializeField]
    private Sprite _androidImage;
    private Image _image;

    private void Awake() {
        if (_image == null)
            _image = GetComponent<Image>();
#if UNITY_IOS
        _image.sprite = _appleImage;
#else
        _image.sprite = _androidImage;
#endif
    }
}
