using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage tutorial
/// </summary>
public class ARMessageTurorialBtn : MonoBehaviour {

    private PermissionController _permissionController;

    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        permissionController.CheckCameraMicAccess(() => {
            SettingsConstructor.OnActivated?.Invoke(false);
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            StreamCallBacks.onCloseComments?.Invoke();
            ARMessageTutorialConstructor.OnActivated?.Invoke(true);
            ARMessageRoomConstructor.OnActivated?.Invoke(false);
        });
    }
}
