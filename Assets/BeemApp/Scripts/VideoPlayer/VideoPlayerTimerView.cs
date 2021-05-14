using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerTimerView : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    private Text timerText;

    private void Awake()
    {
        timerText = GetComponent<Text>();
    }

    private void Update()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(videoPlayer.frame);
        timerText.text = string.Concat(timeSpan);
    }
}
