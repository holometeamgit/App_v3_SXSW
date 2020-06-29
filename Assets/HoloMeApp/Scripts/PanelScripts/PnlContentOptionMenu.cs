using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PnlContentOptionMenu : MonoBehaviour
{
    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    UnityEvent OnDeletePerformance;

    public void FlagPerformance() {
        pnlGenericError.ActivateSingleButton("Flag performance", "If you have any concerns with something you’ve seen, please let us know so we can take appropriate action <b>support@holo.me</b> ", onBackPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
    }

    public void DeletePerformance() {
        pnlGenericError.ActivateDoubleButton("Disconnect from live stream?", "Closing this page will disconnect you from the live stream", onButtonOnePress: () => { OnDelete(); }, onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
    }

    private void OnDelete() {
        OnDeletePerformance.Invoke();
    }
}
