using UnityEngine;
using Beem.ARMsg;
using UnityEngine.Events;

/// <summary>
/// UIRecordController. Can request callbacks for start and stor record
/// </summary>
public class UIRecordController : MonoBehaviour
{
    public UnityEvent OnStopped;
    private bool _tryInterrupt;

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

    public void Interrupt() {
        _tryInterrupt = true;
        CallBacks.OnStopRecord?.Invoke();
    }

    private void OnEnable() {
        CallBacks.OnVideoReadyPlay += OnRecordStopped;
    }

    private void OnRecordStopped() {
        if(!_tryInterrupt)
            OnStopped?.Invoke();
        _tryInterrupt = false;
    }

    private void OnDisable() {
        CallBacks.OnVideoReadyPlay -= OnRecordStopped;
    }
}
