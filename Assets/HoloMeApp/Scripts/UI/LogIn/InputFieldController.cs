﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldController : MonoBehaviour
{
    public bool IsClearOnDisable = true;

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

    [SerializeField] TMP_InputField inputField;

    [SerializeField] TMP_Text warningMsgText;
    [SerializeField] GameObject warningMsgRect;
    [SerializeField] GameObject warningOutline;

    public void ShowWarning(string warningMsg) {
        warningMsgText.text = warningMsg;
        warningMsgRect.SetActive(true);
        warningOutline.SetActive(true);
        StartCoroutine(UpdateLayout());
    }

    public void SetToDefaultState() {
        warningMsgRect.SetActive(false);
        warningOutline.SetActive(false);
    }

    public void SetPasswordContentType(bool value) {
        inputField.contentType = value ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }

    private void OnDisable() {
        if(IsClearOnDisable)
            SetToDefaultState();
    }
    //TODO remove after adding animation
    private IEnumerator UpdateLayout() {
        yield return new WaitForEndOfFrame();
        inputField.GetComponent<Image>().enabled = false;
        yield return new WaitForEndOfFrame();
        inputField.GetComponent<Image>().enabled = true;
    }
}