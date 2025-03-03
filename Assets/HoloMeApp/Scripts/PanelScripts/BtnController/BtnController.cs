﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// the class is responsible for the events that occur with the button when the user interacts with it
//  for example checks the interactivity of a button
/// </summary>

public class BtnController : MonoBehaviour {
    private Button _btn;

    public UnityEvent OnPress;

    private HashSet<Func<bool>> _onNeedCheckInteractionRequirement;

    private void Awake() {
        _onNeedCheckInteractionRequirement = new HashSet<Func<bool>>();
    }

    /// <summary>
    /// Adds a requirements method to the set
    /// </summary>
    /// <param name="onCheckRequirement"></param>
    public void AddCheckInteractionRequirementListener(Func<bool> onCheckRequirement) {
        InitSetRequirements();
        _onNeedCheckInteractionRequirement.Add(onCheckRequirement);
    }
    /// <summary>
    /// remove a requirements method to the set
    /// </summary>
    /// <param name="onCheckRequirement"></param>
    public void RemoveCheckInteractionRequirementListener(Func<bool> onCheckRequirement) {
        InitSetRequirements();
        _onNeedCheckInteractionRequirement.Remove(onCheckRequirement);
    }

    /// <summary>
    /// In the future, according to a variety of requirements,
    /// it can determine whether the button is now interactive or not
    /// </summary>
    public void CheckInteractionRequirement(bool available = true) {
        if (_btn == null) {
            _btn = GetComponent<Button>();
        }

        foreach (var isMetRequirements in _onNeedCheckInteractionRequirement) {
            if (!isMetRequirements.Invoke()) {
                _btn.interactable = false;
                return;
            }
        }

        _btn.interactable = available;
    }

    /// <summary>
    /// Invole press the btn
    /// </summary>
    public void BtnPress() {
        OnPress.Invoke();
    }

    private void InitSetRequirements() {
        if (_onNeedCheckInteractionRequirement == null)
            _onNeedCheckInteractionRequirement = new HashSet<Func<bool>>();
    }

}
