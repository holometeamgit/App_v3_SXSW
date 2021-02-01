using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Email View
    /// </summary>
    public class ProfileProfileNameView : AbstractProfileView {

        [Header("Profile ProfileName")]
        [SerializeField]
        private Text text;

        public override void Refresh(FirebaseUser firebaseUser) {
            text.text = firebaseUser.DisplayName;
        }
    }
}
