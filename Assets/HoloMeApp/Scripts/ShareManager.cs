using UnityEngine;
using NatShare;
using Beem.Firebase.DynamicLink;
using System;

public class ShareManager : MonoBehaviour {
    public void ShareStream() {
        ShareStream(string.Empty);
    }

    [ContextMenu("ShareRoomStream")]
    public void ShareRoomStream() {
        HelperFunctions.DevLog("ShareRoomStream");
        StreamCallBacks.onGetMyRoomLink?.Invoke();
    }

    private void Awake() {
        DynamicLinksCallBacks.onGetShortLink += ShareMyRoomLink;
    }

    private void OnDestroy() {
        DynamicLinksCallBacks.onGetShortLink -= ShareMyRoomLink;
    }

    private void ShareStream(string aditionalInformation) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);

        new NativeShare().SetText(aditionalInformation).Share();

        //using (var payload = new SharePayload()) {
        //    payload.AddText(aditionalInformation);
        //}
    }

    private void ShareMyRoomLink(string link) {
        string msg = "Come to my room: " + link;
        HelperFunctions.DevLog(msg);
#if !UNITY_EDITOR
        ShareStream(link);
#endif
    }

    private void ShareMyRoomLink(Uri link) {
        ShareMyRoomLink(link.AbsoluteUri);
    }
}
