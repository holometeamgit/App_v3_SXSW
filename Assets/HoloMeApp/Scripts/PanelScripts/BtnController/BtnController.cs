using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class BtnController : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] float _delayAfterClick = 1;

    private HashSet<Func <bool>> _onNeedCheckInteractionRequirement;

    public UnityEvent OnPress;

    public void AddCheckInteractionRequirementListener(Func<bool> onCheckRequirement) {
        _onNeedCheckInteractionRequirement.Add(onCheckRequirement);
    }

    public void RemoveCheckInteractionRequirementListener(Func<bool> onCheckRequirement) {
        _onNeedCheckInteractionRequirement.Remove(onCheckRequirement);
    }

    public void CheckInteractionRequirement() {
        foreach(var checker in _onNeedCheckInteractionRequirement) {
            if(checker.Invoke()) {

            }
        }
    }

    public void BtnPress() {
        OnPress.Invoke();
    }

}
