using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Email View
    /// </summary>
    public class ProfileEmailView : AbstractProfileView {

        [Header("Profile email")]
        [SerializeField]
        private Text text;

        public override void Refresh(FirebaseUser firebaseUser) {
            text.text = firebaseUser.Email;
        }
    }
}
