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
        #region SSO
        public static Action<string, string> onSignInEMail = delegate { };
        public static Action onSignInEMailClick = delegate { };
        public static Action onSignInGoogle = delegate { };
        public static Action onSignInApple = delegate { };
        public static Action onSignInFacebook = delegate { };
        public static Action onSignInSuccess = delegate { };
        public static Action<LogInType> onFirebaseSignInSuccess = delegate { };

        public static Action<string> onNeedVerification = delegate { };

        public static Action<string, string, string, string> onSignUp = delegate { };
        public static Action onSignUpEMailClick = delegate { };
        public static Action onSignUpSuccess = delegate { };

        public static Action onResetPasswordClick = delegate { };
        public static Action onResetPasswordMsgSent = delegate { };
        public static Action<string> onForgotAccount = delegate { };

        public static Action onEmailVerification = delegate { };

        public static Action onSignOutClick = delegate { };
        public static Action onSignOut = delegate { };

        public static Action onUserWasDeleted = delegate { };

        public static Action<string> onFail = delegate { };
        #endregion

        //TODO move to other place
        #region purchase
        public static Action<long> onStreamPurchasedInStore = delegate { };
        public static Action<long> onStreamPurchasedAndUpdateOnServer = delegate { };

        #endregion
    }
}