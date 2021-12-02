using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Constructor for ar
/// </summary>
public class ARConstructor : MonoBehaviour {

    [SerializeField]
    private GameObject arSessionOrigin;
    [SerializeField]
    private GameObject arSession;

    public static Action<bool> onActivated = delegate { };

    private void OnEnable() {
        onActivated += Activate;
    }

    private void OnDisable() {
        onActivated -= Activate;
    }

    private void Activate(bool status) {
        HelperFunctions.DevLogError($"AR activated: {status}");
        arSessionOrigin?.SetActive(status);
        arSession?.SetActive(status);
    }

}
