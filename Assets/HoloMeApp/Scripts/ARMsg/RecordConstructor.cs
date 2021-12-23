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

    // Start is called before the first frame update
    private void Start() {
        CallBacks.OnStartRecord += _recordButton.StartAnimation;
    }

    private void OnDestroy() {
        CallBacks.OnStartRecord -= _recordButton.StartAnimation;
    }
}
