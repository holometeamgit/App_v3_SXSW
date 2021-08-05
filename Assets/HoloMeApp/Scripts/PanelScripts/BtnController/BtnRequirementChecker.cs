using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BtnController))]
public class BtnInteractionRequirementChecker : MonoBehaviour {
    protected BtnController _btnController;
    protected bool _canInteract;
    /// <summary>
    /// if all requirements are met, return true
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckRequirements() {
        return true;
    }

    protected virtual void Awake() {
        if (_btnController == null)
            _btnController = GetComponent<BtnController>();
    }
}

public interface BtnInterface {
    public bool CheckRequirements1();
}