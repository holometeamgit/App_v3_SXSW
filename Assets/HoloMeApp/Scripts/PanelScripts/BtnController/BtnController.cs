using System.Collections;
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

    /// <summary>
    /// In the future, according to a variety of requirements,
    /// it can determine whether the button is now interactive or not
    /// </summary>
    public void CheckInteractionRequirement(bool available = true) {
        if (_btn == null) {
            _btn = GetComponent<Button>();
        }

        _btn.interactable = available;
    }

    /// <summary>
    /// Invole press the btn
    /// </summary>
    public void BtnPress() {
        OnPress.Invoke();
    }

}
