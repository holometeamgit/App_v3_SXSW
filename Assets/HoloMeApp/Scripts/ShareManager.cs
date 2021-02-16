using UnityEngine;
using NatShare;

public class ShareManager : MonoBehaviour
{
    public void ShareStream() {
        ShareStream(string.Empty);
    }

    [ContextMenu("ShareRoomStream")]
    public void ShareRoomStream() {
        StreamCallBacks.onGetMyRoomLink?.Invoke();
    }

    private void Awake() {
        StreamCallBacks.onMyRoomLinkReceived += ShareMyRoomLink;
    }

    private void ShareStream(string aditionalInformation) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);

        using (var payload = new SharePayload()) {
            string appName = "Beem";
            string iosLink = "https://apps.apple.com/us/app/beem/id1532446771?ign-mpt=uo%3D2";
            string androidLink = "https://play.google.com/store/apps/details?id=com.HoloMe.Beem";
            string appLink;
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    appLink = iosLink;
                    break;

                case RuntimePlatform.Android:
                    appLink = androidLink;
                    break;

                default:
                    appLink = iosLink + " - " + androidLink;
                    break;
            }

            string message = $"Click the link below to download the {appName} app which lets you experience human holograms using augmented reality: ";
            payload.AddText(message + appLink + aditionalInformation);
        }
    }

    private void ShareMyRoomLink(string link) {
        string msg = "Come to my room: " + link;
        HelperFunctions.DevLog(msg);
#if !UNITY_EDITOR
        ShareStream(msg);
#endif
    }
}
