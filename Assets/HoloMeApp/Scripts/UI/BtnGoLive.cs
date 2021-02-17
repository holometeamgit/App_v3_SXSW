using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class BtnGoLive : MonoBehaviour
{
    [SerializeField] UnityEvent OnGoLive;
    [SerializeField] PermissionController permissionController;

    public void GoLive() {
        if (permissionController.CheckCameraMicAccess())
            OnGoLive.Invoke();
    }
}
