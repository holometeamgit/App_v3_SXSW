using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class PnlChannelName : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputChannelName;

    [SerializeField]
    TextMeshProUGUI txtInvalidNameWarning;

    [SerializeField]
    TextMeshProUGUI txtEnterChannelName;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    public static string ChannelName { get; private set; }

    public void OnReadyPressed()
    {
        //Any verification and validation should go here
        if (string.IsNullOrWhiteSpace(inputChannelName.text))
        {
            StartCoroutine(InvalidNameRoutine());
        }
        else
        {
            ChannelName = inputChannelName.text;
            OnChannelNamePassed?.Invoke();
        }
    }

    private void OnEnable()
    {
        inputChannelName.text = "";
        inputChannelName.interactable = true;
        ToggleIncorrectCodeText(false);
    }

    public void IncorrectCodeShake()
    {
        txtInvalidNameWarning.GetComponent<RectTransform>().DOShakeAnchorPos(.25f, new Vector3(40, 0, 0), 80);
    }

    public void ToggleIncorrectCodeText(bool show)
    {
        txtInvalidNameWarning.gameObject.SetActive(show);
        txtEnterChannelName.gameObject.SetActive(!show);
    }

    IEnumerator InvalidNameRoutine()
    {
        ToggleIncorrectCodeText(true);
        IncorrectCodeShake();
        inputChannelName.interactable = false;
        yield return new WaitForSeconds(2);
        inputChannelName.text = "";
        inputChannelName.interactable = true;
        ToggleIncorrectCodeText(false);
    }

}
