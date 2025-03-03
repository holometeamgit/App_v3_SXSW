using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Abstract class for button interactivity checkers
/// </summary>
[RequireComponent(typeof(BtnController))]
public abstract class BtnInteractionRequirementChecker : MonoBehaviour {
    protected BtnController _btnController;
    protected bool _canInteract = true;

    /// <summary>
    /// This action is fired when the interactivity requirements have been updated.
    /// </summary>
    protected Action<bool> _onAvailableUpdated;
    /// <summary>
    /// The function is needed to check the requirements for the interactivity of the button
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckRequirements();

    protected void CheckBtnController() {
        if (_btnController == null)
            _btnController = GetComponent<BtnController>();
    }

    protected virtual void OnEnable() {
        CheckBtnController();
        _btnController.AddCheckInteractionRequirementListener(CheckRequirements);
        _onAvailableUpdated += _btnController.CheckInteractionRequirement;
    }

    protected virtual void OnDisable() {
        _btnController.RemoveCheckInteractionRequirementListener(CheckRequirements);
        _onAvailableUpdated -= _btnController.CheckInteractionRequirement;
    }
}