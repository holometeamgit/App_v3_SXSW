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

        HelperFunctions.DevLog("check room ShowBroadcastHoldingScreen");
        if (isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            SwitcherToRoomBroadcastHoldingScreen.Switch();
        } else if(isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Stream)) {
            long id = 0;
            if (!long.TryParse(contentLinkHandler.PopContentId(), out id))
                return;

            if (isHomePageActive)
                pnlThumbnailPopup.OpenStream(id);
        } else if(homePageSwitcher ) {
            if (!pnlStreamOverlay.isActiveAndEnabled) {
                HelperFunctions.DevLog("Open HomePage ");
                homePageSwitcher.Switch();
            } else {
                HelperFunctions.DevLog("Can't open home page because stream ");
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
