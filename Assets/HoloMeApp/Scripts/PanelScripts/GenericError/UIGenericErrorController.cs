using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

public class UIGenericErrorController : MonoBehaviour {
    [SerializeField] PnlGenericError pnlGenericError;

    // Start is called before the first frame update
    void Awake() {
        CallBacks.OnActivateGenericErrorDoubleButton += OnActivateDoubleButton;
        CallBacks.OnCloseGenericError += OnCloseGenericError;
    }

    private void OnActivateDoubleButton(string header = "", string message = "",
        string buttonOneText = "Yes", string buttonTwoText = "No",
        UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null,
        bool isWarning = false) {
        pnlGenericError.ActivateDoubleButton(header, message,
            buttonOneText, buttonTwoText,
            onButtonOnePress, onButtonTwoPress, isWarning);
    }

    private void OnCloseGenericError() {
        pnlGenericError.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        CallBacks.OnActivateGenericErrorDoubleButton -= OnActivateDoubleButton;
        CallBacks.OnCloseGenericError -= OnCloseGenericError;
    }
}