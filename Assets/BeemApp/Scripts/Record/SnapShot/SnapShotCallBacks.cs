using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Record.SnapShot {

    /// <summary>
    /// SnapshotCallbacks
    /// </summary>
    public class SnapShotCallBacks {
        public static Action onSnapshotStarted = delegate { };
        public static Action<Texture2D> onSnapshotEnded = delegate { };
        public static Action onPostRecord = delegate { };
    }
}
