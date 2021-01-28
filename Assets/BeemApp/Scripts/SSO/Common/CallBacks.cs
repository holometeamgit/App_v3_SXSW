using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Callbacks for SignIn
    /// </summary>
    public class CallBacks {
        public static Action<string, string> onSignInMail = delegate { };
        public static Action onSignInGoogle = delegate { };
        public static Action onSignInApple = delegate { };
        public static Action onSignInFacebook = delegate { };

        public static Action onSignInSuccess = delegate { };
        public static Action<string, string, string, string> onSignUp = delegate { };
        public static Action onSignUpSuccess = delegate { };
        public static Action<string> onFail = delegate { };
        public static Action<string> onForgotAccount = delegate { };
        public static Action onSignOut = delegate { };
    }
}