using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldInterationController : MonoBehaviour
{
    [SerializeField]
    Image _outlineImg;

    public void OnSelected() {
        _outlineImg.enabled = true;
    }

    public void OnDeselected() {
        _outlineImg.enabled = false;
    }

    private void OnEnable() {
        _outlineImg.enabled = false;
    }

    private void OnDisable() {
        _outlineImg.enabled = false;
    }
}
