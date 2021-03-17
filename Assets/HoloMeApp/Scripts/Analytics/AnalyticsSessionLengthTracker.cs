using UnityEngine;

public class AnalyticsSessionLengthTracker : MonoBehaviour {
    const string sessionLength = nameof(sessionLength);

    private void Start() {
        DontDestroyOnLoad(this);
        AnalyticsController.Instance.StartTimer(sessionLength, AnalyticKeys.KeySessionLength);
    }

    private void OnApplicationQuit() {
        AnalyticsController.Instance.StopTimer(sessionLength);
    }

}
