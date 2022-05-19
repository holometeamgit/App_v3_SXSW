using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

/// <summary>
/// Call event for upload logo 
/// </summary>
public class BtnUploadLogo : MonoBehaviour {
    public void UploadPhoto() {
        CallBacks.onUploadSelectedLogo?.Invoke();
    }
}
