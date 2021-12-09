using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatSuite.Examples.Components;
using Beem.ARMsg;

public class RecordConstructor : MonoBehaviour
{
    [SerializeField] CircleButtonWithTimer _recordButton;

    // Start is called before the first frame update
    void Start() {
        CallBacks.OnStartRecord += _recordButton.StartAnimation;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= _recordButton.StartAnimation;
    }
}
