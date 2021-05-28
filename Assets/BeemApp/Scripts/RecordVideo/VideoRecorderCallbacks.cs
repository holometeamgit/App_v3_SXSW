using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoRecorderCallbacks {
    public static Action onStartRecording = delegate { };
    public static Action onStopRecording = delegate { };
    public static Action onSnapshotStarted = delegate { };
    public static Action onSnapshotEnded = delegate { };
}
