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

        public static Action onUserDataLoaded = delegate { };

        public static Action onUserWasDeleted = delegate { };

        public static Action<string> onFail = delegate { };

        //requests
        public static Action onQuickLogInRequest = delegate { };
        public static Action onLogOutRequest = delegate { };

        #endregion

        //TODO move to other place
        #region purchase
        public static Action<long> onStreamPurchasedInStore = delegate { };
        public static Action<long> onStreamPurchasedAndUpdateOnServer = delegate { };

        #endregion

        #region like
        // action parameter is stream id 
        public static Action<long> onClickLike = delegate { };
        public static Action<long> onSentRequestLikeCallback = delegate { };
        public static Action<long> onFailSentRequestLikeCallback = delegate { };

        public static Action<long> onClickUnlike = delegate { };
        public static Action<long> onSentRequestUnlikeCallback = delegate { };
        public static Action<long> onFailSentRequestUnlikeCallback = delegate { };

        public static Action<long> onGetLikeState = delegate { };
        public static Action<long, bool, long> onGetLikeStateCallBack = delegate { };

        #endregion

        #region view
        public static Action<long> onViewed = delegate { };
        public static Action<long> onSentRequestViewCallback = delegate { };
        public static Action<long> onFailSentRequestViewCallback = delegate { };

        #endregion

        #region streams
        public static Action onStreamsContainerUpdated = delegate { };
        public static Action<long> onStreamByIdInContainerUpdated = delegate { };

        public static Action<long> onDownloadStreamById = delegate { };
        #endregion

        #region business account
        public static Action onBusinessLogoLoaded;
        public static Action onSelectLogoFromDevice;
        public static Action onLogoSelected;
        public static Action onUploadSelectedLogo;
        public static Action onRemoveLogo;

        public static Action onLogoUploaded;
        public static Action onLogoUploadingError;

        public static Func<bool> hasLogoOnDevice;
        public static Func<Sprite> getLogoOnDevice;
        //return logo just selected but not set like logo on the server
        public static Func<Sprite> getSelectedLogoOnDevice; 

        #endregion
    }
}