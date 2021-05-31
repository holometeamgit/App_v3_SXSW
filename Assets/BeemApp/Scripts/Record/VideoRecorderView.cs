using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoRecorderView : MonoBehaviour {

    [SerializeField]
    private Image progressBar;

    private void OnEnable() {
        OnProgress();
        VideoRecorderCallbacks.onProgressRecording += OnProgress;
    }

    private void OnDisable() {
        VideoRecorderCallbacks.onProgressRecording -= OnProgress;
    }

    private void OnProgress(float value = 0f) {
        if (progressBar != null) {
            progressBar.fillAmount = value;
        }
    }
}
