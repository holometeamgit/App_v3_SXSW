using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapShotCallBacks {
    public static Action onSnapshotStarted = delegate { };
    public static Action<Sprite> onSnapshotEnded = delegate { };
}
