using Beem.Permissions;
using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBtn : MonoBehaviour, IARMsgDataView {

    private ARMsgJSON.Data _arMsgData = default;

    private PermissionController _permissionController = new PermissionController();

    public void Init(ARMsgJSON.Data arMsgData) {
        _arMsgData = arMsgData;
    }

    /// <summary>
    /// Open AR Messages
    /// </summary>
    public void Open() {
        if (_arMsgData.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            _permissionController.CheckCameraMicAccess(() => {
                ARMsgRecordConstructor.OnActivated?.Invoke(false);
                ARenaConstructor.onActivateForARMessaging?.Invoke(_arMsgData);
                ARMsgARenaConstructor.OnActivatedARena?.Invoke(_arMsgData);
                GalleryConstructor.OnHide?.Invoke();
                PnlRecord.CurrentUser = _arMsgData.user;
            });
        }
    }
}
