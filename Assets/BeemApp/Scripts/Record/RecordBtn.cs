using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


public class RecordBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [Header("Recording Time")]
    [SerializeField]
    private Vector2 recordingTime = new Vector2(2, 15);

    private float timer = 0;

    private float Timer {
        get {
            return timer;
        }
        set {
            if (Mathf.Abs(timer - value) > Mathf.Epsilon) {
                timer = value;
                VideoRecordCallbacks.onRecordProgress?.Invoke(timer / (recordingTime.y));
            }
        }
    }

    private CancellationTokenSource cancelTokenSource;
    private bool pressed = false;

    public void OnPointerDown(PointerEventData eventData) {
        pressed = true;
        Record();
    }

    public void OnPointerUp(PointerEventData eventData) {
        pressed = false;
    }

    public async void Record() {
        cancelTokenSource = new CancellationTokenSource();
        try {
            VideoRecordCallbacks.onRecordStarted?.Invoke();
            Timer = 0;
            float startTime = Time.time;
            while (Timer < recordingTime.y && pressed) {
                Timer = Time.time - startTime;
                await Task.Yield();
            }
            if (Timer < recordingTime.x) {
                VideoRecordCallbacks.onRecordFailed?.Invoke();
            } else {
                VideoRecordCallbacks.onRecordStoped?.Invoke();
            }
            Timer = 0;
        } finally {
            if (cancelTokenSource != null) {
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }

    public void Cancel() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }
}
