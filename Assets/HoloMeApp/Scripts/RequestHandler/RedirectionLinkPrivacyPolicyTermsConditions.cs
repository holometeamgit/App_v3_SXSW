using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectionLinkPrivacyPolicyTermsConditions : MonoBehaviour
{
    [SerializeField] ExternalLinksScriptableObject externalLinksScriptableObject;

    public void Redirect() {
        Application.OpenURL(externalLinksScriptableObject.PrivacyPolicyTermsConditions);
    }
}
