using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScrollSnapComponent : MonoBehaviour {

    private ScrollSnap scrollSnap;

    private void OnEnable() {
        scrollSnap = GetComponentInParent<ScrollSnap>();
        scrollSnap.onProgress.AddListener(OnProgress);
        scrollSnap.onLerpComplete.AddListener(OnLerpCompleted);
        scrollSnap.onRelease.AddListener(OnRelease);
    }

    private void OnDisable() {
        scrollSnap.onProgress.RemoveListener(OnProgress);
        scrollSnap.onLerpComplete.RemoveListener(OnLerpCompleted);
        scrollSnap.onRelease.RemoveListener(OnRelease);
    }

    public void OnRelease(int value) {
        Debug.LogError($"OnRelease = {value}");
    }

    public void OnLerpCompleted() {
        Debug.LogError($"OnLerpCompleted");
    }

    public void OnProgress(Vector2 value) {
        Debug.LogError($"OnProgress = {value}");
    }
}
