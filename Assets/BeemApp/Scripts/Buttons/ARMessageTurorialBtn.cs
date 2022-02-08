using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage tutorial
/// </summary>
public class ARMessageTurorialBtn : MonoBehaviour {

    private PermissionController _permissionController = new PermissionController();

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        _permissionController.CheckCameraMicAccess(() => {
            SettingsConstructor.OnActivated?.Invoke(false);
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            StreamCallBacks.onCloseComments?.Invoke();
            ARMessageTutorialConstructor.OnActivated?.Invoke(true);
        });
    }
}
