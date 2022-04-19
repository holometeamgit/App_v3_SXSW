using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open/Close Business Options Btn
/// </summary>
public class BusinessOptionsBtn : MonoBehaviour {

    [SerializeField]
    private bool isOpened;

    /// <summary>
    /// Open/Close Business Options
    /// </summary>
    public void OnClick() {
        if (isOpened) {
            BusinessOptionsConstructor.OnShowLast?.Invoke();
        } else {
            BusinessOptionsConstructor.OnHide?.Invoke();
        }
    }
}
