using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Constructor for Generic Error Pnl
/// </summary>
public class GenericConstructor : MonoBehaviour {
    [SerializeField]
    private PnlGenericError pnlGenericError;

    public static event Action<string, string, string, UnityAction, bool> onActivateSingleButton = delegate { };
    public static event Action<string, string, string, string, UnityAction, UnityAction, bool> onActivateDoubleButton = delegate { };
    public static event Action onDeactivate = delegate { };

    private void OnEnable() {
        onActivateSingleButton += OnActivateSingleButton;
        onActivateDoubleButton += OnActivateDoubleButton;
        onDeactivate += OnDeactivate;
    }

    private void OnDisable() {
        onActivateSingleButton -= OnActivateSingleButton;
        onActivateDoubleButton -= OnActivateDoubleButton;
        onDeactivate -= OnDeactivate;
    }

    private void OnActivateSingleButton(string header = "", string message = "", string buttonText = "Back", UnityAction onBackPress = null, bool isWarning = false) {
        pnlGenericError.ActivateSingleButton(header, message, buttonText, onBackPress, isWarning);
    }

    private void OnActivateDoubleButton(string header = "", string message = "", string buttonOneText = "Yes", string buttonTwoText = "No", UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null, bool isWarning = false) {
        pnlGenericError.ActivateDoubleButton(header, message, buttonOneText, buttonTwoText, onButtonOnePress, onButtonTwoPress, isWarning);
    }

    private void OnDeactivate() {
        pnlGenericError.Deactivate();
    }

    /// <summary>
    /// Activate Single Btn Error
    /// </summary>
    /// <param name="header"></param>
    /// <param name="message"></param>
    /// <param name="buttonText"></param>
    /// <param name="onBackPress"></param>
    /// <param name="isWarning"></param>
    public static void ActivateSingleButton(string header = "", string message = "", string buttonText = "Back", UnityAction onBackPress = null, bool isWarning = false) {
        onActivateSingleButton?.Invoke(header, message, buttonText, onBackPress, isWarning);
    }

    /// <summary>
    /// Activate Double Btn Error
    /// </summary>
    /// <param name="header"></param>
    /// <param name="message"></param>
    /// <param name="buttonOneText"></param>
    /// <param name="buttonTwoText"></param>
    /// <param name="onButtonOnePress"></param>
    /// <param name="onButtonTwoPress"></param>
    /// <param name="isWarning"></param>
    public static void ActivateDoubleButton(string header = "", string message = "", string buttonOneText = "Yes", string buttonTwoText = "No", UnityAction onButtonOnePress = null, UnityAction onButtonTwoPress = null, bool isWarning = false) {
        onActivateDoubleButton?.Invoke(header, message, buttonOneText, buttonTwoText, onButtonOnePress, onButtonTwoPress, isWarning);
    }

    /// <summary>
    /// Deacivate pnl
    /// </summary>
    public static void Deactivate() {
        onDeactivate?.Invoke();
    }
}
