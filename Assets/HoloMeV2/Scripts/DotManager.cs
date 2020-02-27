using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DotManager : MonoBehaviour
{
    [SerializeField]
    Transform[] dotTransforms;

    [SerializeField]
    Transform[] cursorTransforms;

    [SerializeField]
    Sprite spriteDot;

    [SerializeField]
    Sprite spriteWrongCode;

    [SerializeField]
    TMP_InputField inputText;

    public Action<string> OnPassCodeEntered;

    private void Start()
    {
        PassCodeChanged();
    }

    public void ActivateTextField()
    {
        inputText.ActivateInputField();
    }

    public void PassCodeChanged()
    {
        for (int i = 0; i < dotTransforms.Length; i++)
        {
            var lengthOfString = inputText.text.ToCharArray().Length - 1;
            dotTransforms[i].gameObject.SetActive(lengthOfString >= i);
        }

        for (int i = 0; i < cursorTransforms.Length; i++)
        {
            cursorTransforms[i].gameObject.SetActive(false);
        }

        int index = (inputText.text.ToCharArray().Length - 1) + 1;
        if (index <= cursorTransforms.Length - 1)
        {
            cursorTransforms[index].gameObject.SetActive(true);
        }
    }

    public void ToggleBoxSprites(bool incorrectCode)
    {
        for (int i = 0; i < dotTransforms.Length; i++)
        {
            dotTransforms[i].GetComponent<Image>().sprite = incorrectCode ? spriteWrongCode : spriteDot;
        }
    }

    public void TryCode()
    {
        if (inputText.text.ToCharArray().Length == 4)
        {
            OnPassCodeEntered?.Invoke(inputText.text);
        }
    }

    public void DisableInputText()
    {
        inputText.interactable = false;
    }

    public void EnableInputText()
    {
        inputText.interactable = true;
    }

    public void ClearText()
    {
        inputText.text = "";
    }
}
