using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

public class UITextField : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    private Text textComponent;
    [SerializeField]
    private GameObject placeHolder;

    public InputField.ContentType contentType;
    public InputField.InputType inputType;
    public InputField.LineType lineType;
    public TouchScreenKeyboardType keyboardType;
    public InputField.CharacterValidation characterValidation;
    public int characterLimit;

    [SerializeField]
    private UnityEvent onClick;

    private void Awake() {
        text = string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke();
    }

    public string text {
        get {
            return textComponent.text;
        }

        set {
            textComponent.text = value;
            placeHolder.SetActive(string.IsNullOrEmpty(textComponent.text));
        }

    }



}
