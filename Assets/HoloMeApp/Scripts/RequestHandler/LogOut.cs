using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    [SerializeField] PnlGenericError pnlGenericError;
    [SerializeField] Switcher switcher;

    public void DoLogOut() {
        pnlGenericError.ActivateDoubleButton(null, "Are you sure you want to log out?",
            "log out", "Cancel",
            () => switcher.Switch(), pnlGenericError.CancelInvoke);
    }
}
