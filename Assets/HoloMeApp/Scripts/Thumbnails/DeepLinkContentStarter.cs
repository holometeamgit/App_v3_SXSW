using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkContentStarter : MonoBehaviour
{
    [SerializeField]
    Switcher SwitcherToRoomBroadcastHoldingScreen;
    [SerializeField]
    ContentLinkHandler contentLinkHandler;
    [SerializeField]
    VideoUploader videoUploader;
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    PnlThumbnailPopup pnlThumbnailPopup;
    [SerializeField]
    Switcher homePageSwitcher;
    [SerializeField]
    PnlStreamOverlay pnlStreamOverlay;
    [SerializeField]
    PnlRoomBroadcastHoldingScreen _pnlRoomBroadcastHoldingScreen;
    [SerializeField]
    GameObject homeScreen;

    bool isHomePageActive;

    private float DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN = 0.5f;

    //calling from scene on HomeScreen Game Object (Enabler script)
    public void OnHomePageEnable() {
        isHomePageActive = true;
        StartCoroutine(ShowBroadcastHoldingScreenWithDelay());
    }

    //calling from scene on HomeScreen Game Object (Enabler script)
    public void OnHomePageDisable() {
        isHomePageActive = false;
        StopAllCoroutines();
    }

    private void ShowContent() {
        if (!accountManager.IsAuthorized()) {
            HelperFunctions.DevLog("Can't open room because user it not LogIn");
            return;
        }

        if (isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            HelperFunctions.DevLog("Has room deeplink. Switch to waiting room screen");
            SwitcherToRoomBroadcastHoldingScreen.Switch();
        } else if(isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Stream)) {
            HelperFunctions.DevLog("Has streem deeplink");
            long id = 0;
            if (!long.TryParse(contentLinkHandler.PopContentId(), out id)) {
                HelperFunctions.DevLog("incorrect streem deeplink ID");
                return;
            }

            if (isHomePageActive)
                pnlThumbnailPopup.OpenStream(id);
        } else if(homePageSwitcher) {
            if (!pnlStreamOverlay.isActiveAndEnabled && !_pnlRoomBroadcastHoldingScreen.isActiveAndEnabled) {
                HelperFunctions.DevLog("App doesn't have deeplink. Home page will open");
                homePageSwitcher.Switch();
            } else {
                HelperFunctions.DevLog("Can't open home page because stream is open");
                homeScreen.SetActive(false);
            }
        }
    }

    private void OnEnable() {
        StreamCallBacks.onOpenRoom += ShowContent;
        StreamCallBacks.onOpenStream += ShowContent;
    }

    private void OnDisable() {
        StreamCallBacks.onOpenRoom -= ShowContent;
        StreamCallBacks.onOpenStream -= ShowContent;
    }

    private IEnumerator ShowBroadcastHoldingScreenWithDelay() {
        yield return new WaitForSeconds(DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN);

        if (isHomePageActive)
            ShowContent();
    }
}
