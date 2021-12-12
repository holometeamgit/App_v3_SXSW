using UnityEngine;
using Beem.ARMsg;

/// <summary>
/// UIRecordController. Can request callbacks for start and stor record
/// </summary>
public class UIRecordController : MonoBehaviour
{

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
}
