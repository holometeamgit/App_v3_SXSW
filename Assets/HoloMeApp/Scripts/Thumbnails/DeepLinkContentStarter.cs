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
        HelperFunctions.DevLog("check room ShowBroadcastHoldingScreen");
        HelperFunctions.DevLog("isHomePageActive " + isHomePageActive);
        if (isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            HelperFunctions.DevLog("room ShowBroadcastHoldingScreen");
            SwitcherToRoomBroadcastHoldingScreen.Switch();
        } else if(isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Stream)) {
            long id = 0;
            HelperFunctions.DevLog("try parse PopContentId");
            if (!long.TryParse(contentLinkHandler.PopContentId(), out id))
                return;

            if (isHomePageActive)
                pnlThumbnailPopup.OpenStream(id);
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
