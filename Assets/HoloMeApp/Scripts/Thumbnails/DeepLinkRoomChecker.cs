using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkRoomChecker : MonoBehaviour
{
    [SerializeField] GameObject RoomBroadcastHoldingScreen;
    [SerializeField] RoomLinkHandler roomLinkHandler;

    private void ShowBroadcastHoldingScreen() {
        if(gameObject.activeInHierarchy)
            RoomBroadcastHoldingScreen.SetActive(roomLinkHandler.HasRoomId());
    }

    private void OnEnable() {
        ShowBroadcastHoldingScreen();
        StreamCallBacks.onOpenRoom += ShowBroadcastHoldingScreen;
    }

    private void OnDisable() {
        StreamCallBacks.onOpenRoom -= ShowBroadcastHoldingScreen;
    }
}
