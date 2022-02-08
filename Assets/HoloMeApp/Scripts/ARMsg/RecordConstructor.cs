using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatSuite.Examples.Components;
using Beem.ARMsg;

/// <summary>
/// RecordConstructor. Subscribe start animation recording 
/// </summary>
public class RecordConstructor : MonoBehaviour
{
    [SerializeField]
    private CircleButtonWithTimer _recordButton;
    private RecordController _recordController;

    // Start is called before the first frame update
    private void Start() {
        CallBacks.OnStartRecord += _recordButton.StartAnimation;
        CallBacks.onRecordTimerSet += _recordButton.SetMaxRecordingTime;

        _recordController = new RecordController();
        CallBacks.onSwitchRecordTimerClicked += _recordController.SwitchTimer;
        CallBacks.onGetCurrevRecordTimerClicked += _recordController.OnGetCurrentRecordTimer;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= _recordButton.StartAnimation;
        CallBacks.onRecordTimerSet -= _recordButton.SetMaxRecordingTime;
        CallBacks.onSwitchRecordTimerClicked += _recordController.SwitchTimer;
        CallBacks.onGetCurrevRecordTimerClicked += _recordController.OnGetCurrentRecordTimer;
    }
}
