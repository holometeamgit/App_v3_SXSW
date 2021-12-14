using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOut : MonoBehaviour {
    [SerializeField] Switcher switcher;

    public void DoLogOut() {
        WarningConstructor.ActivateDoubleButton(null, "Are you sure you want to log out?",
            "log out", "Cancel",
            () => switcher.Switch());
    }
}
