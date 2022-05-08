using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class BtnUploadLogo : MonoBehaviour {
    public void UploadPhoto() {
        CallBacks.onUploadSelectedLogo?.Invoke();
    }
}
