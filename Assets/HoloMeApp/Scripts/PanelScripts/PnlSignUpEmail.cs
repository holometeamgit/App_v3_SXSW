using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlSignUpEmail : MonoBehaviour
{
    [SerializeField] EmailAccountManager emailAccountManager;
    [SerializeField] AccountManager accountManager;
    [SerializeField] PnlEmailVerification pnlEmailVerification;
    [SerializeField] Switcher switcherToVerification;
//    [SerializeField] InputFieldController inputFieldFullName;
    [SerializeField] InputFieldController inputFieldEmail;
    [SerializeField] InputFieldController inputFieldPassword;
    [SerializeField] InputFieldController inputFieldConfirmPassword;

    public void SignUp() {
        EmailSignUpJsonData emailSignUpJsonData = new EmailSignUpJsonData();
        emailSignUpJsonData.email = inputFieldEmail.text;
//        emailSignUpJsonData.username = inputFieldFullName.text;
        emailSignUpJsonData.password1 = inputFieldPassword.text;
        emailSignUpJsonData.password2 = inputFieldConfirmPassword.text;

        emailAccountManager.SignUp(emailSignUpJsonData, SignUpCallBack, ErrorSignUpCallBack);
    }

    void Start() {
        
    }

    private void SignUpCallBack(long code, string body) {
        Debug.Log(code + " : " + body);
        pnlEmailVerification.SetActionOnVerified(() => accountManager.SaveAccessToken(body));
        switcherToVerification.Switch();
    }

    private void ErrorSignUpCallBack(long code, string body) {

        BadRequestSignUpEmailJsonData badRequestData = JsonUtility.FromJson<BadRequestSignUpEmailJsonData>(body);

        Debug.Log(code + " : " + body);
//        if (badRequestData.username.Count > 0)
//            inputFieldFullName.ShowWarning(badRequestData.username[0]);
        //Debug.Log(badRequestData.username[0]);
        if (badRequestData.email.Count > 0)
            inputFieldEmail.ShowWarning(badRequestData.email[0]);
            //Debug.Log(badRequestData.email[0]);
        if (badRequestData.password1.Count > 0)
            inputFieldPassword.ShowWarning(badRequestData.password1[0]);
            //Debug.Log(badRequestData.password1[0]);
        if (badRequestData.non_field_errors.Count > 0)
            inputFieldConfirmPassword.ShowWarning(badRequestData.non_field_errors[0]);
            //Debug.Log(badRequestData.non_field_errors[0]);
    }


}
