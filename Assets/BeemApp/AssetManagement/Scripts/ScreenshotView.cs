using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ScreenshotView : MonoBehaviour {
    [SerializeField]
    private VideoPlayer _videoPlayer;
    [SerializeField]
    private int _maxHeight = 345;
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action onFail) {
        int videoWidth;
        int videoHeight;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out videoWidth, out videoHeight, _maxHeight);

        _videoPlayer.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(videoWidth, videoHeight);

        _videoPlayer.url = data.ar_message_s3_link;

        if (!_videoPlayer.isPrepared) {
            onFail?.Invoke();
            _videoPlayer.Prepare();
            _videoPlayer.prepareCompleted +=
                (player) => {
                    onSuccess?.Invoke();
                    _videoPlayer.Play();
                    _videoPlayer.Pause();
                };
        } else {
            onSuccess?.Invoke();
        }
    }

}
