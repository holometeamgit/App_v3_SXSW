using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PnlContentOptionMenu : MonoBehaviour {

    [SerializeField]
    UnityEvent OnDeletePerformance;

    public void FlagPerformance() {
        PnlGenericErrorConstructor.ActivateSingleButton("Flag performance",
            "If you have any concerns with something you’ve seen, please let us know so we can take appropriate action <b>support@holo.me</b> ",
            onBackPress: () => PnlGenericErrorConstructor.Deactivate());
    }

    public void DeletePerformance() {
        PnlGenericErrorConstructor.ActivateDoubleButton("Disconnect from live stream?", "Closing this page will disconnect you from the live stream",
            onButtonOnePress: () => { OnDelete(); },
            onButtonTwoPress: () => PnlGenericErrorConstructor.Deactivate());
    }

    private void OnDelete() {
        OnDeletePerformance.Invoke();
    }
}
