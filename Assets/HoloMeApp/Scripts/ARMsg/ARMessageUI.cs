using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// ARMessageUI. Select current step when ARmsg openning
/// </summary>
public class ARMessageUI : MonoBehaviour {
    [SerializeField]
    private GameObject RecordSteps;

    private void OnEnable() {
        RecordSteps.gameObject.SetActive(true);
    }

    /// <summary>
    /// Close ARMessage Steps
    /// </summary>
    public void CloseARMessageSteps() {
        CallBacks.OnCancelAllARMsgActions?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
        ARMsgRecordConstructor.OnActivated?.Invoke(false);
    }

    public void ShowInfoPopupBeemMe() {
        InfoPopupConstructor.onActivate("HOW TO RECORD \n YOUR HOLOGRAM \n MESSAGE", false, PnlInfoPopupColour.Orange);
    }
}
