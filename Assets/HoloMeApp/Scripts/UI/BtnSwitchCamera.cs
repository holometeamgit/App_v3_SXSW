using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;
using UnityEngine.UI;

public class BtnSwitchCamera : MonoBehaviour
{
    [SerializeField]
    private Button _switchBtn;

    private void OnEnable() {
        _switchBtn.interactable = CallBacks.onCanSwitchCamera?.Invoke() ?? false;
    }

    public void SwitchCamera() {
        CallBacks.onSwitchCameraClicked?.Invoke();
    }
}
