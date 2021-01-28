using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
namespace Beem.SSO {
    /// <summary>
    /// ProfileFacade
    /// </summary>
    public class ProfileFacade : MonoBehaviour {

        [Header("Abstract View for UserData")]
        [SerializeField]
        private List<AbstractProfileView> abstractProfileViews = new List<AbstractProfileView>();

        private FirebaseAuth _auth;

        private void OnEnable() {
            Initialize();
        }

        private void Initialize() {
            _auth = FirebaseAuth.DefaultInstance;
            _auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        private void OnDisable() {
            if (_auth != null) {
                _auth.StateChanged -= AuthStateChanged;
            }
        }

        private void RefreshView(FirebaseUser firebaseUser) {
            foreach (AbstractProfileView item in abstractProfileViews) {
                item.Refresh(firebaseUser);
            }
        }

        private void AuthStateChanged(object sender, System.EventArgs eventArgs) {
            bool signedIn = _auth.CurrentUser != null;
            if (signedIn) {
                RefreshView(_auth.CurrentUser);
            }
        }
    }
}
