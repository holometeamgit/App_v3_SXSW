using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     
/// </summary>
[RequireComponent(typeof(BtnController))]
public abstract class BtnInteractionRequirementChecker : MonoBehaviour {
    protected BtnController _btnController;
    protected bool _canInteract = true;

    /// <summary>
    /// This action is fired when the interactivity requirements have been updated.
    /// </summary>
    protected Action _onRequirementsUpdated;
    /// <summary>
    /// The function is needed to check the requirements for the interactivity of the button
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckRequirements();

    protected virtual void Awake() {;
        if (_btnController == null)
            _btnController = GetComponent<BtnController>();
    }

    protected virtual void OnEnable() {
        _btnController.AddCheckInteractionRequirementListener(CheckRequirements);
        _onRequirementsUpdated += _btnController.CheckInteractionRequirement;
    }

    protected virtual void OnDisable() {
        _btnController.RemoveCheckInteractionRequirementListener(CheckRequirements);
        _onRequirementsUpdated -= _btnController.CheckInteractionRequirement;
    }

    private void InitBtnController() {

    }
}