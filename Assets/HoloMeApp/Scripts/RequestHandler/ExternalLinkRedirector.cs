using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalLinkRedirector : MonoBehaviour
{
    //TODO inject via DI
    [SerializeField] ExternalLinksScriptableObject externalLinksScriptableObject;
    [SerializeField] ExternalLinksScriptableObject.ExternalLinkType externalLinkType;

    public void Redirect() {
        Application.OpenURL(externalLinksScriptableObject.GetLink(externalLinkType));
    }
}
