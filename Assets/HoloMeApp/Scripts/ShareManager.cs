using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatShare;

public class ShareManager : MonoBehaviour
{
    public void ShareStream() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);

        using (var payload = new SharePayload()) {
            string appName = "Beem";
            string iosLink = "https://apps.apple.com/us/app/beem/id1532446771?ign-mpt=uo%3D2";
            string androidLink = "https://play.google.com/store/apps/details?id=com.HoloMe.Beem";
            string appLink;
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    appLink = iosLink;
                    appName = "Beem+";
                    break;

                case RuntimePlatform.Android:
                    appLink = androidLink;
                    appName = "Beem";
                    break;

                default:
                    appLink = iosLink + " - " + androidLink;
                    break;
            }

            string message = $"Click the link below to download the {appName} app which lets you experience human holograms using augmented reality: ";
            payload.AddText(message + appLink);
        }
    }

}
