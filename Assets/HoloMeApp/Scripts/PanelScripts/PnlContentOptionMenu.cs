using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PnlContentOptionMenu : MonoBehaviour {

    [SerializeField]
    UnityEvent OnDeletePerformance;

    public void FlagPerformance() {
        WarningConstructor.ActivateSingleButton("Flag performance",
            "If you have any concerns with something you’ve seen, please let us know so we can take appropriate action <b>support@holo.me</b> ",
            onBackPress: () => WarningConstructor.Deactivate());
    }

    public void DeletePerformance() {
        WarningConstructor.ActivateDoubleButton("Disconnect from live stream?", "Closing this page will disconnect you from the live stream",
            onButtonOnePress: () => { OnDelete(); },
            onButtonTwoPress: () => WarningConstructor.Deactivate());
    }

    private void OnDelete() {
        OnDeletePerformance.Invoke();
    }
}
