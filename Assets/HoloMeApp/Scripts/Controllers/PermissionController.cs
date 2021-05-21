﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID || UNITY_ANDROID
using UnityEngine.Android;
#endif

public class PermissionController : MonoBehaviour {

    [SerializeField] PermissionGranter permissionGranter;
    [SerializeField] PnlGenericError pnlGenericError;
    
    public bool CheckCameraMicAccess() {
        return CheckCameraAccess() && CheckMicAccess();
    }

    public bool CheckCameraAccess() {
        if (permissionGranter.HasCameraAccess)
            return true;

        pnlGenericError.ActivateDoubleButton("Camera Access Required!",
            "Please enable camera access to use this app",
            "Settings",
            "Cancel",
            () => permissionGranter.RequestCameraAccess(),
            () => pnlGenericError.gameObject.SetActive(false));

        return false;
    }

    public bool CheckMicAccess() {
        if (permissionGranter.MicAccessAvailable)
            return true;

        if (permissionGranter.MicRequestComplete)
            permissionGranter.RequestMicAccess();
        else
            pnlGenericError.ActivateDoubleButton("Mic Access Required!",
                "Please enable mic access to use this app",
                "Settings",
                "Cancel",
                () => permissionGranter.RequestMicAccess(),
                () => pnlGenericError.gameObject.SetActive(false));

        return false;
    }

    public void CheckSettings(string settingsTitle, string settingsDescription) {
            pnlGenericError.ActivateDoubleButton(settingsTitle,
                settingsDescription,
                "Allow",
                "Don’t Allow",
                () => permissionGranter.RequestSettings(),
                () => pnlGenericError.gameObject.SetActive(false));
    }

    public void CheckPushNotifications() {
        HelperFunctions.DevLog("CheckPushNotifications permission");
        CheckSettings("‘Beem’ Would Like To Send You Notifications", "Notifications may include alerts and sounds, and icon badget. These can be configure in Settings.");
    }
}
