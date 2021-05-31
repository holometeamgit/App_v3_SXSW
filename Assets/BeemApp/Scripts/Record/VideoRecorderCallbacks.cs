using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoRecorderCallbacks {
    public static Action onStartRecording = delegate { };
    public static Action<float> onProgressRecording = delegate { };
    public static Action onStopRecording = delegate { };
}
