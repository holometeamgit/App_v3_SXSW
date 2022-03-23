using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Mopsicus.Plugins;
using System.Linq;

public class InputFieldController : MonoBehaviour {
    public bool IsClearOnDisable = true;
    public bool IsLowercase;

    [SerializeField]
    private bool isEmail;
    [SerializeField]
    private bool isTrim;

    [SerializeField]
    InputField inputField;

    [SerializeField]
    private MobileInputField _mobileInputField;

    public MobileInputField MobileInputField {
        get {
            return _mobileInputField;
        }
    }

    [SerializeField]
    TMP_Text warningMsgText;
    [SerializeField]
    Animator animator;

    private bool showWarning;

    [HideInInspector]
    public UnityEvent onValueChanged;
    public UnityEvent OnEndEditPassword;

    public int characterLimit {
        get { return inputField.characterLimit; }
        set { inputField.characterLimit = value; }
    }

    public string text {
        get {
            if (_mobileInputField != null && _mobileInputField.InputField != null) {
                return _mobileInputField.Text;
            } else {
                return inputField.text;
            }
        }
        set {
            if (_mobileInputField != null && _mobileInputField.InputField != null) {
                _mobileInputField.Text = value;
            } else {
                inputField.text = value;
            }
        }
    }

    public bool interactable {
        get { return inputField.interactable; }
        set { inputField.interactable = value; }
    }

    private void Awake() {
        inputField.onEndEdit.AddListener(DoOnEndEditPassword);
        inputField.onValueChanged.AddListener(OnValueChanged);
        if (_mobileInputField != null) {
            _mobileInputField.OnReturnPressedEvent.AddListener(OnReturn);
        }
        if (isEmail) {
            inputField.contentType = InputField.ContentType.EmailAddress;
        }
    }

    public void ShowWarning(string warningMsg) {
        if (!string.IsNullOrEmpty(warningMsg)) {
            animator.enabled = true;
            HelperFunctions.DevLog(warningMsg);
            warningMsgText.text = OverrideMsg(warningMsg);
            showWarning = true;
            animator.SetBool("ShowWarning", showWarning);
        }
    }

    public void SetToDefaultState() {
        if (!showWarning)
            return;
        showWarning = false;
        animator.SetBool("ShowWarning", showWarning);
    }

    public void DisableAnimator() {
        animator.keepAnimatorControllerStateOnDisable = false;
        animator.enabled = false;
    }

    public void SetPasswordContentType(bool value) {
        inputField.contentType = value ? InputField.ContentType.Standard : InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            OnReturn();
        }
    }

    private string OverrideMsg(string msg) {
        //sign up
        HelperFunctions.DevLog("OverrideMsg " + msg);
        if (msg.Contains("is exist"))
            return "Already exists";

        if (msg.Contains("EmailAlreadyInUse"))
            return "Email already exists";

        if (msg.Contains("A user is already registered with this e-mail address."))
            return "Email is already in use";

        if (msg.Contains("Enter a valid email address") || msg.Contains("MissingEmail") || msg.Contains("InvalidEmail"))
            return "Enter a valid email address";

        //password
        if (msg.Contains("This password is too short. It must contain at least 8 characters") ||
            msg.Contains("This password is too common") ||
            msg.Contains("This password is entirely numeric") ||
            msg.Contains("The password is too similar to the username") ||
            msg.Contains("WeakPassword"))
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

        if (msg.Contains("TooManyRequest"))
            return "Too many request";

        return msg;
    }

    private void DoOnEndEditPassword(string value = "") {
        if (isTrim) {
            text = RegexAlphaNumeric.RegexResult(text);
        }
        if (IsLowercase) {
            text = text.ToLower();
        }
        OnEndEditPassword.Invoke();
    }

    private void OnValueChanged(string value) {
        onValueChanged.Invoke();
    }

    private void OnReturn() {
        inputField.onEndEdit?.Invoke(inputField.text);
    }

    private void OnDestroy() {
        inputField.onEndEdit.RemoveListener(DoOnEndEditPassword);
        inputField.onValueChanged.RemoveListener(OnValueChanged);
        if (_mobileInputField != null) {
            _mobileInputField.OnReturnPressedEvent.RemoveListener(OnReturn);
        }
    }

    private void OnDisable() {
        if (IsClearOnDisable) {
            SetToDefaultState();
            text = "";
            DisableAnimator();
        }
    }
}