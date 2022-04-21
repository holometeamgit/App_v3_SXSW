using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindOptionsWindow : MonoBehaviour {
    [SerializeField]
    private Mover _mover;

    public void Show() {
        gameObject.SetActive(true);
        _mover.ChangeState(true);
        _mover.onEndMoving += OnClose;
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _mover.ChangeState(false);
    }

    private void OnClose(bool status) {
        if (!status) {
            _mover.onEndMoving -= OnClose;
            gameObject.SetActive(false);
            BusinessOptionsConstructor.OnHide?.Invoke();
            CTALinkOptionsConstructor.OnHide?.Invoke();
            SuccessOptionsConstructor.OnHide?.Invoke();
        }
    }
}
