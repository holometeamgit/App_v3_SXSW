using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class BtnChooseFromPhotos : MonoBehaviour {
    public void ChoosePhoto() {
        CallBacks.onUpdateLogoFromDevice?.Invoke();
    }
}
