using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open/Close CTALinkOptions Btn
/// </summary>
public class CTALinkOptionsBtn : MonoBehaviour {

    [SerializeField]
    private bool isOpened;

    /// <summary>
    /// Open/Close CTALink Options
    /// </summary>
    public void OnClick() {
        if (isOpened) {
            CTALinkOptionsConstructor.OnShow?.Invoke();
        } else {
            CTALinkOptionsConstructor.OnHide?.Invoke();
        }
    }
}
