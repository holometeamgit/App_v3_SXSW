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
        Support
    }

    public string PrivacyPolicyTermsConditions = "https://holo.me/help/";
    public string PrivacyPolicy = "https://holo.me/help/";
    public string TermsConditions = "https://holo.me/help/";
    public string Support = "https://holo.me/help/";

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
            default:
                return "";
        }
    }
}
