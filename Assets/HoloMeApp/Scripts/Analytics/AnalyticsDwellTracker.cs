using UnityEngine;

public class AnalyticsDwellTracker : MonoBehaviour {
    string trackerName;
    public int Timer { get; private set; }

    public void StartTimer() {
        Timer = 0;
        InvokeRepeating(nameof(IncrementSeconds), 1, 1);
    }

    public void PauseTimer() {
        CancelInvoke(nameof(IncrementSeconds));
    }

    public void ResumeTimer() {
        InvokeRepeating(nameof(IncrementSeconds), 1, 1);
    }

    public void StopTimer() {
        CancelInvoke(nameof(IncrementSeconds));
    }

    void IncrementSeconds() {
        Timer++;
    }
}
