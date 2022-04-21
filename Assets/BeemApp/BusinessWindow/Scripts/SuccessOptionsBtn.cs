using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open/Close SuccessOptions Btn
/// </summary>
public class SuccessOptionsBtn : MonoBehaviour {

    [SerializeField]
    private bool isOpened;

    /// <summary>
    /// Open/Close SuccessOptions
    /// </summary>
    public void OnClick() {
        if (isOpened) {
            SuccessOptionsConstructor.OnShow?.Invoke();
        } else {
            SuccessOptionsConstructor.OnHide?.Invoke();
        }
    }
}
