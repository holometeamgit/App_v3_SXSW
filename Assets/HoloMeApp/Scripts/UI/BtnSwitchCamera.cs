using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;
using UnityEngine.UI;

/// <summary>
/// View for switch camera (front/back)
/// </summary>
public class BtnSwitchCamera : MonoBehaviour {
    [SerializeField]
    private Button _switchBtn;

    private void OnEnable() {
        StartCoroutine(DelayedInteractableCheck());
    }

    private IEnumerator DelayedInteractableCheck() {
        yield return new WaitForEndOfFrame();
        _switchBtn.interactable = CallBacks.onCanSwitchCamera?.Invoke() ?? false;
    }

    /// <summary>
    /// Call switch camera
    /// </summary>
    public void SwitchCamera() {
        CallBacks.onSwitchCameraClicked?.Invoke();
    }
}
