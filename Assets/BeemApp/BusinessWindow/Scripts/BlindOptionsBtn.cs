using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open/Close Blind Options Btn
/// </summary>
public class BlindOptionsBtn : MonoBehaviour {

    [SerializeField]
    private string _assetId;

    [SerializeField]
    private bool isOpened;

    /// <summary>
    /// Open/Close Blind Options
    /// </summary>
    public void OnClick() {
        if (isOpened) {
            BlindOptionsConstructor.Show(_assetId);
        } else {
            BlindOptionsConstructor.Hide();
        }
    }
}
