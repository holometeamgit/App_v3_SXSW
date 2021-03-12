using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkRoomChecker : MonoBehaviour
{
    [SerializeField] Switcher SwitcherToRoomBroadcastHoldingScreen;
    [SerializeField] RoomLinkHandler roomLinkHandler;

    private float DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN = 0.5f;

    private void ShowBroadcastHoldingScreen() {
        HelperFunctions.DevLog("check ShowBroadcastHoldingScreen");
        if (gameObject.activeInHierarchy && roomLinkHandler.HasRoomId()) {
            SwitcherToRoomBroadcastHoldingScreen.Switch();

            HelperFunctions.DevLog("ShowBroadcastHoldingScreen");
        }
    }

    private void OnEnable() {
        StartCoroutine(ShowBroadcastHoldingScreenWithDelay());
        StreamCallBacks.onOpenRoom += ShowBroadcastHoldingScreen;
    }

    private void OnDisable() {
        StopAllCoroutines();
        StreamCallBacks.onOpenRoom -= ShowBroadcastHoldingScreen;
    }

    private IEnumerator ShowBroadcastHoldingScreenWithDelay() {
        yield return new WaitForSeconds(DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN);

        if (gameObject.activeInHierarchy)
            ShowBroadcastHoldingScreen();
    }
}
