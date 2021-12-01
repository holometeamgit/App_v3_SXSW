﻿using System;
using Firebase.DynamicLinks;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// Abstract view for Get Link
    /// </summary>
    public abstract class AbstractDynamicLinksView : MonoBehaviour {

        protected void OnEnable() {
            Subscribe();
        }

        protected void OnDisable() {
            Unsubscribe();
        }

        protected virtual void Subscribe() {
            DynamicLinksCallBacks.onShareSocialLink += Refresh;
        }

        protected virtual void Unsubscribe() {
            DynamicLinksCallBacks.onShareSocialLink -= Refresh;
        }

        protected abstract void Refresh(Uri value, SocialMetaTagParameters socialMetaTagParameters);

    }
}
