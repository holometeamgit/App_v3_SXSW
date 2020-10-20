using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsSessionLengthTracker : MonoBehaviour {
    const string sessionLength = nameof(sessionLength);

    private void Awake() {
        DontDestroyOnLoad(this);
        AnalyticsController.Instance.StartTimer(sessionLength, "Session Length");
    }

    private void OnApplicationQuit() {
        AnalyticsController.Instance.StopTimer(sessionLength);
    }

}
