using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// View for Stream Timer
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class StreamTimerView : MonoBehaviour {

    private TMP_Text _timerText;

    private bool active;

    private void Awake() {
        _timerText = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        OnStreamViewClear();
    }

    private void OnDisable() {
        OnStreamViewClear();
    }

    /// <summary>
    /// Show stream timer
    /// </summary>
    /// <param name="data"></param>
    public void OnStreamView(StreamJsonData.Data data) {
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
    public async void OnLiveStreamView(StreamJsonData.Data data) {
        active = true;
        while (active) {
            TimerTextFormat(DateTime.Now - DateTime.Parse(data.start_date));
            await Task.Yield();
        }
        _timerText.text = string.Empty;
    }

    /// <summary>
    /// Date from prerecorded;
    /// </summary>
    /// <param name="data"></param>
    public void OnPrerecordedStreamView(StreamJsonData.Data data) {
        TimerTextFormat(TimeSpan.FromSeconds(data.duration));
    }

    private void TimerTextFormat(TimeSpan timeSpan) {
        if (timeSpan.TotalSeconds > 0) {
            _timerText.text = timeSpan.Hours > 0 ? string.Format("{0}h {1}m", timeSpan.Hours, timeSpan.Minutes) : string.Format("{0}m", timeSpan.Minutes);
        } else {
            _timerText.text = string.Empty;
        }
    }

    /// <summary>
    /// Clear Stream Timer Info
    /// </summary>
    public void OnStreamViewClear() {
        active = false;
        _timerText.text = string.Empty;
    }

}
