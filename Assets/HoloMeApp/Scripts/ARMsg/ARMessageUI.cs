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

    private const string KEY_SEEN_TUTORIAL_BEEMME = nameof(KEY_SEEN_TUTORIAL_BEEMME);

    /// <summary>
    /// Reopen
    /// </summary>
    public void Reopen() {
        CallBacks.OnCancelAllARMsgActions?.Invoke();
        ARMsgRecordConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// ShowInfoPopupBeemMe
    /// </summary>
    public void ShowInfoPopupBeemMe() {
        InfoPopupConstructor.onActivate("HOW TO RECORD \n YOUR HOLOGRAM \n MESSAGE", false, PnlInfoPopupColour.Orange);
    }

    private void ShowInfoPopUpFirstTime() {
        if (PlayerPrefs.GetString(KEY_SEEN_TUTORIAL_BEEMME, "") == "") {
            PlayerPrefs.SetString(KEY_SEEN_TUTORIAL_BEEMME, KEY_SEEN_TUTORIAL_BEEMME);
            ShowInfoPopupBeemMe();
        }
    }

    private void OnEnable() {
        RecordSteps.gameObject.SetActive(true);
        ShowInfoPopUpFirstTime();
        MenuConstructor.OnActivated?.Invoke(true);
    }

    private void OnDisable() {
        CallBacks.OnCancelAllARMsgActions?.Invoke();
    }
}
