using Beem.ARMsg;
using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage
/// </summary>
public class ARMessageBtn : MonoBehaviour {
    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
        ARMsgRecordConstructor.OnActivated?.Invoke(true);
    }
}
