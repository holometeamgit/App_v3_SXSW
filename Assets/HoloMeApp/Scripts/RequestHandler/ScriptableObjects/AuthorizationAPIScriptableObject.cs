﻿/*
 * This script contain request API string for user authorization.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/")]
[CreateAssetMenu(fileName = "AuthorizationAPI", menuName = "Data/API/AuthorizationAPI", order = 101)]
public class AuthorizationAPIScriptableObject : ScriptableObject {
    [Header("Email Registration")]
    [Tooltip("Post registration request by mail. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Register/post_signup_")]
    public string EmailSignUp = "/signup/";
    [Tooltip("Post email verification request. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Register/post_signup_verify_email_")]
    public string EmailVerification = "/signup/verify-email/";
    [Tooltip("Post resend verification request. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Register/post_signup_resend_verification_email_")]
    public string ResendVerification = "/signup/resend-verify-email/";

    [Space]
    [Tooltip("Post request to get access token using email LogIn. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Token/post_token_")]
    public string EmailLogIn = "/token/";

    [Header("Password changes")]
    [Tooltip("Post request change password after authorization. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Password/post_password_change_")]
    public string ChangePassword = "/password/change/";

    [Space]
    [Tooltip("Post request reset password before authorization if this email is in the system. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Password/post_password_reset_")]
    public string ResetPassword = "/password/reset/";
    [Tooltip("Post request confirm reset password. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Password/post_password_reset_confirm_")]
    public string ResetPasswordConfirm = "/password/reset/confirm/";

    [Header("Refresh token")]
    [Tooltip("Post request refresh token. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Token/post_token_refresh_")]
    public string RefreshToken = "/token/refresh/";

    [Header("Firebase Authorization")]
    [Tooltip("Get request complite Log In. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/Token/post_token_firebase_")]
    public string FirebaseToken = "/token/firebase/";

    [Header("User")]
    [Tooltip("Get request user data. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/User/get_user_")]
    public string GetUser = "/user/";
    [Tooltip("Put request add user data. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/User/put_user_")]
    public string PutUser = "/user/";
    [Tooltip("Delete request user data. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/User/delete_user_")]
    public string DeleteUser = "/user/";
    [Tooltip("Delete request disable user account and delete profile info. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/User/delete_user_disable_")]
    public string DisableUser = "/user/disable/";
    [Tooltip("Post request upload profile pictures. Details here: https://devholo.me/docs/auth/?urls.primaryName=Auth%20Gateway#/User/post_user_profile_picture_")]
    public string ProfilePictures = "/user/disable/";

    [Header("Profile")]
    public string GetMyProfile = "/b_profile/me/";
    public string UpdateLogo = "/b_profile/{id}/logo/update/";
    public string DefaultLogoLink = "https://ar.beem.me/06f014a34d4c92d9a4ac.png";
}
