using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.ARMsg;
using TMPro;

/// <summary>
/// View for showing timer for Beem Me feature
/// </summary>
public class BtnSwitchRecordTimer : MonoBehaviour {
    [SerializeField]
    private TMP_Text _timerValueText;

    private const string SUFFIX = "s";

    /// <summary>
    /// Change Timer value
    /// </summary>
    public void SwitchTimer() {
        CallBacks.onSwitchRecordTimerClicked?.Invoke();
    }

    private void Awake() {
        CallBacks.onRecordTimerSet += OnRecordTimerSwitched;
        CallBacks.onGetCurrevRecordTimerClicked?.Invoke();
    }

    private void OnRecordTimerSwitched(int value) {
        _timerValueText.text = value + SUFFIX;
    }

    private void OnDestroy() {
        CallBacks.onRecordTimerSet -= OnRecordTimerSwitched;
    }
}
