using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InputFieldController : MonoBehaviour
{
    public bool IsClearOnDisable = true;
    public bool IsLowercase;

    public int characterLimit {
        get { return inputField.characterLimit; }
        set { inputField.characterLimit = value; }
    }

    public string text {
        get { return inputField.text; }
        set { inputField.text = value; }
    }

    public bool interactable {
        get { return inputField.interactable; }
        set { inputField.interactable = value; }
    }

    [SerializeField]
    InputField inputField;

    [SerializeField]
    TMP_Text warningMsgText;
    [SerializeField]
    Animator animator;

    [SerializeField] UnityEvent OnEndEditPassword;

    private void Awake() {
        inputField.onEndEdit.AddListener((_) => OnEndEditPassword.Invoke());
        inputField.shouldHideMobileInput = true;
//        inputField.onEndEdit.AddListener(UpdateLayout);
        if (IsLowercase)
            inputField.onValueChanged.AddListener((str) => inputField.text = str.ToLower());
    }

    public void ShowWarning(string warningMsg) {
        HelperFunctions.DevLog(warningMsg);
        warningMsgText.text = OverrideMsg(warningMsg);
        animator.SetBool("ShowWarning", true);
    }

    public void SetToDefaultState() {
        animator.SetBool("ShowWarning", false);
    }

    public void SetPasswordContentType(bool value) {
        inputField.contentType = value ? InputField.ContentType.Standard : InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    private string OverrideMsg(string msg) {
        //sign up
        Debug.Log("OverrideMsg " +  msg);
        if (msg.Contains("is exist") || msg.Contains("EmailAlreadyInUse")) 
            return "Username already exists";

        if (msg.Contains("A user is already registered with this e-mail address."))
            return "Email is already in use";

        if (msg.Contains("Enter a valid email address") || msg.Contains("MissingEmail") || msg.Contains("InvalidEmail"))
            return "Enter a valid email address";

        //password
        if (msg.Contains("This password is too short. It must contain at least 8 characters") ||
            msg.Contains("This password is too common") ||
            msg.Contains("This password is entirely numeric") ||
            msg.Contains("The password is too similar to the username"))
            return "Password must contain letters, numbers \nand be at least 8 characters";

        if (msg.Contains("The two password fields didn’t match") ||
            msg.Contains("The two password fields didn't match")) //if you think that these are the same lines, then you are wrong 
            return "Passwords do not match";

        if (msg.Contains("Wrong password") || msg.Contains("WrongPassword"))
            return "Password is incorrect";

        if (msg.Contains("Account wasn't found") || msg.Contains("UserNotFound"))
            return "Account wasn't found";

        if (msg.Contains("E-mail is not verified"))
            return "E-mail is not verified";

        if (msg.Contains("User account is disabled") || msg.Contains("UserDisabled"))
            return "This account was previously deleted, please contact support to reinstate";

        return msg;
    }

    //private void UpdateLayout(string str) {
    //    inputField.ForceLabelUpdate();
    //}

    private void OnDisable() {
        if (IsClearOnDisable) {
            SetToDefaultState();
            text = "";
        }
    }
}