using UnityEngine;
using Beem.ARMsg;
using UnityEngine.Events;

/// <summary>
/// UIRecordController. Can request callbacks for start and stor record
/// </summary>
public class UIRecordController : MonoBehaviour
{
    public UnityEvent OnStopped;

    /// <summary>
    /// StartRecord
    /// </summary>
    public void StartRecord() {
        CallBacks.OnStartRecord?.Invoke();
    }

    /// <summary>
    /// StopRecord
    /// </summary>
    public void StopRecord() {
        CallBacks.OnStopRecord?.Invoke();
    }

    private void OnEnable() {
        CallBacks.OnVideoReadyPlay += OnRecordStopped;
    }

    private void OnRecordStopped() {
        OnStopped?.Invoke();
    }

    private void OnDisable() {
        CallBacks.OnVideoReadyPlay -= OnRecordStopped;
    }
}
