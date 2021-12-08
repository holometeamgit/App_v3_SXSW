using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatSuite.Examples.Components;
using Beem.ARMsg;

public class RecordConstructor : MonoBehaviour {
    [SerializeField]
    private RecordButtonWithTimer _recordButton;

    // Start is called before the first frame update
    private void Start() {
        CallBacks.OnStartRecord += _recordButton.StartRecordAnimation;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= _recordButton.StartRecordAnimation;
    }
}
