using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;

[System.Serializable]
public class BoolEvent : UnityEvent<bool>
{
}

public class VideoPlayerPlayBtn : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private BoolEvent onPause;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPause?.Invoke(!videoPlayer.isPlaying);
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else {
            videoPlayer.Play();
        }
    }
}
