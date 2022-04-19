using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Options Constructor
/// </summary>
public class CTALinkOptionsConstructor : MonoBehaviour {

    [SerializeField]
    private CTALinkOptionsWindow _ctaLinkOptionsView;

    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show() {
        _ctaLinkOptionsView.Show();
    }

    private void Hide() {
        _ctaLinkOptionsView.Hide();
    }
}
