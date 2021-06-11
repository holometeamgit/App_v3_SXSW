using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExternalLinks", menuName = "Data/API/ExternalLinks", order = 102)]
public class ExternalLinksScriptableObject : ScriptableObject
{
    public enum ExternalLinkType {
        PrivacyPolicyTermsConditions,
        PrivacyPolicy,
        TermsConditions,
        Support,
        Store
    }

    public string PrivacyPolicyTermsConditions = "https://holo.me/help/";
    public string PrivacyPolicy = "https://holo.me/help/";
    public string TermsConditions = "https://holo.me/help/";
    public string Support = "https://holo.me/help/";
    public string AppStoreLink = "https://apps.apple.com/au/app/beem-me/id1532446771";
    public string GoogleStoreLink = "https://play.google.com/store/apps/details?id=com.HoloMe.Beem";

    public string GetLink(ExternalLinkType linkType) {
        switch(linkType) {
            case ExternalLinkType.PrivacyPolicyTermsConditions:
                return PrivacyPolicyTermsConditions;
            case ExternalLinkType.PrivacyPolicy:
                return PrivacyPolicy;
            case ExternalLinkType.TermsConditions:
                return TermsConditions;
            case ExternalLinkType.Support:
                return Support;
            case ExternalLinkType.Store:
#if UNITY_IOS
                return AppStoreLink;
#elif UNITY_ANDROID
                return GoogleStoreLink;
#endif
            default:
                return "";
        }
    }
}
