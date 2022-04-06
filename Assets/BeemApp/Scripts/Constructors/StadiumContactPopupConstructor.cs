using System;
using UnityEngine;

public class StadiumContactPopupConstructor : MonoBehaviour {
    [SerializeField]
    private PnlStadiumContactPopup pnlStadiumContactPopup;

    public static Action<bool> onActivate = delegate { };

    private void OnEnable() {
        onActivate += Activate;
    }

    private void OnDisable() {
        onActivate -= Activate;
    }

    protected void Activate(bool status) {
        if (status) {
            pnlStadiumContactPopup.Activate();
        } else {
            pnlStadiumContactPopup.Hide();
        }
    }
}
