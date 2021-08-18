using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Video Data
/// </summary>
public class PrerecordedVideoData {
    private VideoPlayer _player;
    public VideoPlayer Player {
        get {
            return _player;
        }
    }

    public PrerecordedVideoData(VideoPlayer player) {
        _player = player;
    }
}
