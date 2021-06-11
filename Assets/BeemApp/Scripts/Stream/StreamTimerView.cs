using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// View for Stream Timer
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class StreamTimerView : MonoBehaviour {

    private TMP_Text _timerText;
    private CancellationTokenSource cancelTokenSource;

    private void Awake() {
        _timerText = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Show stream timer
    /// </summary>
    /// <param name="data"></param>
    public void View(StreamJsonData.Data data) {
        switch (data.GetStage()) {
            case StreamJsonData.Data.Stage.Live:
                OnLiveStreamView(data);
                break;
            case StreamJsonData.Data.Stage.Prerecorded:
                OnPrerecordedStreamView(data);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Date from LiveStream
    /// </summary>
    /// <param name="StreamJsonData.Data"></param>
    public void OnLiveStreamView(StreamJsonData.Data data) {
        LiveView(data);
    }

    private async void LiveView(StreamJsonData.Data data) {
        _timerText.text = string.Empty;
        cancelTokenSource = new CancellationTokenSource();
        try {
            while (true) {
                TimerTextFormat(DateTime.Now - DateTime.Parse(data.start_date));
                await Task.Yield();
            }
        }
        finally {
            if (cancelTokenSource != null) {
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }

    private void OnDestroy() {
        Clear();
    }

    /// <summary>
    /// Clear Info
    /// </summary>
    public void Clear() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }

    /// <summary>
    /// Date from prerecorded;
    /// </summary>
    /// <param name="data"></param>
    public void OnPrerecordedStreamView(StreamJsonData.Data data) {
        TimerTextFormat(TimeSpan.FromSeconds(data.duration));
    }

    private void TimerTextFormat(TimeSpan timeSpan) {
        if (timeSpan.TotalMinutes > 0) {
            _timerText.text = timeSpan.Hours > 0 ? string.Format("{0}h {1}m", timeSpan.Hours, timeSpan.Minutes) : string.Format("{0}m", timeSpan.Minutes);
        }
        else {
            _timerText.text = string.Empty;
        }
    }

}
