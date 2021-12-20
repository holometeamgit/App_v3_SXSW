using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScrollSnapElement : MonoBehaviour {

    [SerializeField]
    private int elementNumber;

    private ScrollSnap scrollSnap;

    private void OnEnable() {
        scrollSnap = GetComponentInParent<ScrollSnap>();
        scrollSnap.onProgress.AddListener(OnProgress);
    }

    private void OnDisable() {
        scrollSnap.onProgress.RemoveListener(OnProgress);
    }

    public void OnProgress(float value) {

        Debug.LogError($"gameObject.onFocus = {elementNumber}, value = {value}, Difference = {Mathf.Abs(elementNumber - value)}");
    }
}
