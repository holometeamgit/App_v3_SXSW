using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

/// <summary>
/// Call event for Choose logo 
/// </summary>
public class BtnChooseFromPhotos : MonoBehaviour {
    public void ChoosePhoto() {
        CallBacks.onSelectLogoFromDevice?.Invoke();
    }
}
