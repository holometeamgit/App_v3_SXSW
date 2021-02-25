using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// view in InputField
    /// </summary>
    public class DynamicLinksView : AbstractDynamicLinksView {
        [SerializeField]
        private InputField inputField;

        protected override void Refresh(Uri value) {
            inputField.text = value.AbsoluteUri;
        }
    }
}
