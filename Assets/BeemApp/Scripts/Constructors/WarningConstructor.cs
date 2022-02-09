using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WindowManager.Extenject;

/// <summary>
/// Constructor for Generic Error Pnl
/// </summary>
public class WarningConstructor : MonoBehaviour {
    [SerializeField]
    private PnlWarning pnlGenericError;

    public static Action<GeneralPopUpData> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(GeneralPopUpData data) {
        if (data.FuncBtnData != null) {
            pnlGenericError.ActivateDoubleButton(data.Title, data.Description, data.CloseBtnData.Title, data.FuncBtnData.Title, data.CloseBtnData.OnClick, data.FuncBtnData.OnClick);
        } else {
            pnlGenericError.ActivateSingleButton(data.Title, data.Description, data.CloseBtnData.Title, data.CloseBtnData.OnClick);
        }
    }

    private void Hide() {
        pnlGenericError.Deactivate();
    }
}
