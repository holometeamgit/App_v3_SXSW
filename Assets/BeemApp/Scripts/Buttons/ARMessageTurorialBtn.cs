using Beem;
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
            SettingsConstructor.OnHide?.Invoke();
            HomeConstructor.OnHide?.Invoke();
            BottomMenuConstructor.OnHide?.Invoke();
            CommentsControllersConstructor.OnHide?.Invoke();
            ARMessageTutorialConstructor.OnShow?.Invoke();
        });
    }
}
