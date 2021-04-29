using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        StreamCallBacks.onLiveStreamCreated += OnLiveStreamCreated;
    }

    private void OnDisable()
    {
        StreamCallBacks.onLiveStreamCreated -= OnLiveStreamCreated;
    }

    private void OnLiveStreamCreated(StreamStartResponseJsonData streamStartResponseJsonData) { 
    
    }


}
