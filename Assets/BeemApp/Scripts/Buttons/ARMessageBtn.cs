using Beem.ARMsg;
using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage
/// </summary>
public class ARMessageBtn : MonoBehaviour {

    private PermissionController _permissionController = new PermissionController();

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        _permissionController.CheckCameraMicAccess(() => {
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            ARMessageTutorialConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(true);
        });
    }
}
