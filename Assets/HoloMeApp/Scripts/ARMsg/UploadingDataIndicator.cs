using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.ARMsg;

public class UploadingDataIndicator : MonoBehaviour
{

    [SerializeField] Image _image;
    private float _targetValue;
    private const float LERP_T = 0.15f;

    private void UpdateIndicator(float value) {
        _targetValue = value;
    }

    private void Update() {
        _image.fillAmount = Mathf.Lerp(_image.fillAmount, _targetValue, LERP_T);
    }

    private void OnEnable() {
        _image.fillAmount = 0;
        _targetValue = _image.fillAmount;
        CallBacks.OnARMsgUpdloadedProcessing += UpdateIndicator;
    }

    private void OnDisable() {
        _image.fillAmount = 0;
        _targetValue = _image.fillAmount;
        CallBacks.OnARMsgUpdloadedProcessing -= UpdateIndicator;
    }
}
