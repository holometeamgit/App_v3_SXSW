using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeGalleryNamespace;
using UnityEngine.UI;

public class BtnChangeSplashScreenLogo : MonoBehaviour
{
    public void SelectNewImg() {
        BlindOptionsConstructor.Show("SubpnlChangeLogoWindow");
    }
}