using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;

namespace Beem.SSO {

    /// <summary>
    /// Actions On Sign In And Sign Out
    /// </summary>
    public class AuthManager : MonoBehaviour {

        [Header("SignInAction")]
        [SerializeField]
        private UnityEvent onSignIn;

        [Header("SignOutAction")]
        [SerializeField]
        private UnityEvent onSignOut;

        [Header("SignUpAction")]
        [SerializeField]
        private UnityEvent onSignUp;

        private void OnEnable() {
            CallBacks.onSignInSuccess += SignIn;
            CallBacks.onSignUpSuccess += SignUp;
            CallBacks.onSignOut += SignOut;
        }

        private void OnDisable() {
            CallBacks.onSignInSuccess -= SignIn;
            CallBacks.onSignUpSuccess -= SignUp;
            CallBacks.onSignOut -= SignOut;
        }

        private void SignIn() {
            onSignIn?.Invoke();
        }

        private void SignOut() {
            onSignOut?.Invoke();
        }

        private void SignUp() {
            onSignUp?.Invoke();
        }

    }
}
