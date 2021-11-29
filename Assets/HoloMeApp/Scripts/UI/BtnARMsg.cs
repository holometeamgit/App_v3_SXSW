﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.Permissions;

public class BtnARMsg : MonoBehaviour {
    [SerializeField] UnityEvent OnGo;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

    public void GoARMsg() {
        if (!permissionController.CheckCameraMicAccess()) {
            return;
        }

        OnGo.Invoke();
    }
}
