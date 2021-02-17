using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalLinkRedirector : MonoBehaviour
{
    [SerializeField] ExternalLinksScriptableObject externalLinksScriptableObject;
    [SerializeField] ExternalLinksScriptableObject.ExternalLinkType externalLinkType;

    public void Redirect() {
        Application.OpenURL(externalLinksScriptableObject.GetLink(externalLinkType));
    }
}
