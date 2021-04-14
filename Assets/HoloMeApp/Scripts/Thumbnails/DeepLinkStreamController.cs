﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.Firebase.DynamicLink;

public class DeepLinkStreamController : MonoBehaviour
{
    [SerializeField] ServerURLAPIScriptableObject serverURLAPIScriptableObject;

    private void Awake() {
        StreamCallBacks.onGetStreamLink += GetStreamLink;
    }

    private void GetStreamLink(string id) {
        DynamicLinksCallBacks.onCreateShortLink?.Invoke(serverURLAPIScriptableObject.FirebaseDynamicLink, serverURLAPIScriptableObject.Stream, id, serverURLAPIScriptableObject.Url);
    }

    private void OnDestroy() {
        StreamCallBacks.onGetStreamLink -= GetStreamLink;
    }
}
