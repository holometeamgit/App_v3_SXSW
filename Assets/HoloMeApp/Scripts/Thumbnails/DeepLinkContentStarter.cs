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
    WebRequestHandler webRequestHandler;
    [SerializeField]
    VideoUploader videoUploader;
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    PnlThumbnailPopup pnlThumbnailPopup;

    bool isHomePageActive;

    private float DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN = 0.5f;

    public void OnHomePageEnable() {
        isHomePageActive = true;
        StartCoroutine(ShowBroadcastHoldingScreenWithDelay());
    }

    public void OnHomePageDisable() {
        isHomePageActive = false;
        StopAllCoroutines();
    }

    private void ShowBroadcastHoldingScreen() {
        HelperFunctions.DevLog("check room ShowBroadcastHoldingScreen");
        if (isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Room)) {
            SwitcherToRoomBroadcastHoldingScreen.Switch();

            HelperFunctions.DevLog("room ShowBroadcastHoldingScreen");
        } else if(isHomePageActive && contentLinkHandler.HasContentId(ContentLinkHandlerType.Stream)) {
            long id = 0;
            if (!long.TryParse(contentLinkHandler.PopContentId(), out id))
                return;

            webRequestHandler.GetRequest(GetRequestStreamByIdURL(id),
            (code, body) => {

                StreamJsonData.Data streamJsonData = JsonUtility.FromJson<StreamJsonData.Data>(body);
                if (streamJsonData == null) return;

                if(isHomePageActive)
                    pnlThumbnailPopup.OpenStream(streamJsonData);

           },
       (code, body) => { HelperFunctions.DevLog("Error DownloadStreamById " + id); },
       accountManager.GetAccessToken().access);
        }
    }

    private string GetRequestStreamByIdURL(long id) {
        return webRequestHandler.ServerURLMediaAPI + videoUploader.StreamById.Replace("{id}", id.ToString());
    }

    private void OnEnable() {
        StreamCallBacks.onOpenRoom += ShowBroadcastHoldingScreen;
    }

    private void OnDisable() {
        StreamCallBacks.onOpenRoom -= ShowBroadcastHoldingScreen;
    }

    private IEnumerator ShowBroadcastHoldingScreenWithDelay() {
        yield return new WaitForSeconds(DELAY_TO_SHOW_BROADCASTER_HOLDING_SCREEN);

        if (isHomePageActive)
            ShowBroadcastHoldingScreen();
    }
}
