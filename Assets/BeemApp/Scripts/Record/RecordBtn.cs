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

    private CancellationTokenSource cancelTokenSource;
    private bool pressed = false;

    private PermissionGranter permissionGranter;

    private void Awake() {
        permissionGranter = FindObjectOfType<PermissionGranter>();
    }

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
            float time = 0;
            while (time < recordingTime.y) {
                VideoRecorderCallbacks.onProgressRecording?.Invoke(time / recordingTime.y);
                await Task.Yield();
                if (time < recordingTime.x && !pressed) {
                    VideoRecorderCallbacks.onStopRecording?.Invoke();
                    SnapShotCallBacks.onSnapshotStarted?.Invoke();
                    return;
                }
            }
            VideoRecorderCallbacks.onStopRecording?.Invoke();
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
