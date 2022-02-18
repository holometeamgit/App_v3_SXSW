using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gallery Constructor
/// </summary>
public class GalleryConstructor : MonoBehaviour {

    [SerializeField]
    private GalleryView _galleryView;

    public static Action<ARMsgJSON> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(ARMsgJSON data) {
        _galleryView.Show(data);
    }

    private void Hide() {
        _galleryView.Hide();
    }
}
