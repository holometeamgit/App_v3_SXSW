using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// UIGenericErrorController. Class for invoke UIGenericError
/// </summary>
public class UIGenericErrorController : MonoBehaviour {

    // Start is called before the first frame update
    private void Awake() {
        CallBacks.OnActivateGenericErrorDoubleButton += OnActivateDoubleButton;
    }

    private void OnActivateDoubleButton(string header = "", string message = "",
        string buttonOneText = "Yes", string buttonTwoText = "No",
        UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null,
        bool isWarning = false) {
        GenericConstructor.ActivateDoubleButton(header, message,
            buttonOneText, buttonTwoText,
            onButtonOnePress, onButtonTwoPress, isWarning);
    }

    private void OnDestroy() {
        CallBacks.OnActivateGenericErrorDoubleButton -= OnActivateDoubleButton;
    }
}