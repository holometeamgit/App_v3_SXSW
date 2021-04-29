using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// View for Stream Timer
/// </summary>
public class StreamTimerView : MonoBehaviour
{
    [Header("Text for Timer")]
    [SerializeField]
    private Text timerText;

    private bool active;

    private void OnEnable()
    {
        active = false;
        StreamCallBacks.onLiveStreamCreated += OnLiveStreamCreated;
        StreamCallBacks.onLiveStreamFinished += OnLiveStreamFinish;
    }

    private void OnDisable()
    {
        StreamCallBacks.onLiveStreamCreated -= OnLiveStreamCreated;
        StreamCallBacks.onLiveStreamFinished -= OnLiveStreamFinish;
    }

    private async void OnLiveStreamCreate(StreamStartResponseJsonData streamStartResponseJsonData) {
        active = true;
        while (active) {
            await Task.Yield();
        }
    }

    private void OnLiveStreamFinish() { 
    
    }


}
