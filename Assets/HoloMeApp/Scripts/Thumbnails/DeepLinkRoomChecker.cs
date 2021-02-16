using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkRoomChecker : MonoBehaviour
{
    [SerializeField] Switcher SwitcherToRoomBroadcastHoldingScreen;
    [SerializeField] RoomLinkHandler roomLinkHandler;

    private void ShowBroadcastHoldingScreen() {
        if (gameObject.activeInHierarchy && roomLinkHandler.HasRoomId())
            SwitcherToRoomBroadcastHoldingScreen.Switch();
    }

    private void OnEnable() {
        ShowBroadcastHoldingScreen();
        StreamCallBacks.onOpenRoom += ShowBroadcastHoldingScreen;
    }

    private void OnDisable() {
        StreamCallBacks.onOpenRoom -= ShowBroadcastHoldingScreen;
    }
}
