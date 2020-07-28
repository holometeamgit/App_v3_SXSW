using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PnlChangePassword : MonoBehaviour
{
    [SerializeField] InputFieldController currentPasswordInputField;
    [SerializeField] InputFieldController newPasswordInputField;
    [SerializeField] InputFieldController newPasswordRepeatInputField;

    [SerializeField] PasswordWebManager passwordWebManager;

    public UnityEvent OnChanged;

    public void ChangePassword() {
        PasswordChangeJsonData passwordChangeJsonData = new PasswordChangeJsonData();
        passwordChangeJsonData.old_password = currentPasswordInputField.text;
        passwordChangeJsonData.new_password1 = newPasswordInputField.text;
        passwordChangeJsonData.new_password2 = newPasswordRepeatInputField.text;
        passwordWebManager.ChangePassword(passwordChangeJsonData, PasswordChanedCallBack, PasswordChanedErrorCallBack);
    }

    private void PasswordChanedCallBack(long code, string body) {
        OnChanged.Invoke();
    }

    private void PasswordChanedErrorCallBack(long code, string body) {

        Debug.Log(body);
        try {
            var passwordWarningChangeJsonData = JsonUtility.FromJson<PasswordWarningChangeJsonData>(body);
            if (passwordWarningChangeJsonData.old_password.Count > 0)
                currentPasswordInputField.ShowWarning(passwordWarningChangeJsonData.old_password[0]);
            if (passwordWarningChangeJsonData.new_password1.Count > 0)
                newPasswordInputField.ShowWarning(passwordWarningChangeJsonData.new_password1[0]);
            if (passwordWarningChangeJsonData.new_password2.Count > 0)
                newPasswordRepeatInputField.ShowWarning(passwordWarningChangeJsonData.new_password2[0]);
        } catch (System.Exception e) { Debug.Log("Incorrect: " + body); }
    }
}
