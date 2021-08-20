using System;
using Firebase.DynamicLinks;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// view in InputField
    /// </summary>
    public class DynamicLinksView : AbstractDynamicLinksView {
        [SerializeField]
        private InputField inputField;

        protected override void Refresh(Uri value, SocialMetaTagParameters socialMetaTagParameters) {
            inputField.text = value.AbsoluteUri;
        }
    }
}
