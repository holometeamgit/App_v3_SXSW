using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class IncorrectInputAnimationToggle : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    TextMeshProUGUI txtDefaultMessage;

    [SerializeField]
    TextMeshProUGUI txtErrorMessage;

    private void OnEnable()
    {
        ResetFields();
    }

    /// <param name="OnStart">Action to call before animation begins</param>
    /// <param name="OnComplete">Action to call when animation complets</param>
    public void StartIncorrectAnimation(Action OnStart = null, Action OnComplete = null)
    {
        OnStart?.Invoke();
        StartCoroutine(InvalidNameRoutine(OnComplete));
    }

    void IncorrectCodeShake()
    {
        txtErrorMessage.GetComponent<RectTransform>().DOShakeAnchorPos(.25f, new Vector3(40, 0, 0), 80);
    }

    void ToggleIncorrectCodeText(bool show)
    {
        txtErrorMessage.gameObject.SetActive(show);
        txtDefaultMessage.gameObject.SetActive(!show);
    }

    IEnumerator InvalidNameRoutine(Action OnComplete)
    {
        ToggleIncorrectCodeText(true);
        IncorrectCodeShake();
        inputField.interactable = false;
        yield return new WaitForSeconds(2);
        ResetFields();
        OnComplete?.Invoke();
    }

    private void ResetFields()
    {
        inputField.text = "";
        inputField.interactable = true;
        ToggleIncorrectCodeText(false);
    }
}
