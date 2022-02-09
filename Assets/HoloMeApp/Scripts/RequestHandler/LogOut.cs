using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowManager.Extenject;

public class LogOut : MonoBehaviour {
    [SerializeField] Switcher switcher;

    public void DoLogOut() {

        GeneralPopUpData.ButtonData funcButton = new GeneralPopUpData.ButtonData("Log Out", () => switcher.Switch());
        GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Cancel", null);
        GeneralPopUpData data = new GeneralPopUpData(null, "Are you sure you want to log out?", closeButton, funcButton);

        WarningConstructor.OnShow?.Invoke(data);
    }
}
