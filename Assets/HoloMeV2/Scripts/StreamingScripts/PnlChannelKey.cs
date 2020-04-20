using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PnlChannelKey : MonoBehaviour
{
    [SerializeField]
    DotManager dotManager;

    [SerializeField]
    UnityEvent OnCodePassed;

    [SerializeField]
    TextMeshProUGUI txtIncorrectCode;
    [SerializeField]
    TextMeshProUGUI txtEnterCode;

    private void Start()
    {
        dotManager.OnPassCodeEntered += TestCode;
    }

    private void OnEnable()
    {
        dotManager.ToggleBoxSprites(false);
        dotManager.ClearText();
        ToggleIncorrectCodeText(false);
    }

    void TestCode(string code)
    {
        if (code == "1337")
        {
            OnCodePassed?.Invoke();
        }
        else
        {
            StartCoroutine(WrongCodeRoutine());
        }
    }

    public void IncorrectCodeShake()
    {
        txtIncorrectCode.GetComponent<RectTransform>().DOShakeAnchorPos(.25f, new Vector3(40, 0, 0), 80);
    }

    public void ToggleIncorrectCodeText(bool show)
    {
        txtIncorrectCode.gameObject.SetActive(show);
        txtEnterCode.gameObject.SetActive(!show);
    }

    IEnumerator WrongCodeRoutine()
    {
        ToggleIncorrectCodeText(true);
        IncorrectCodeShake();
        dotManager.ToggleBoxSprites(true);
        yield return new WaitForSeconds(2);
        dotManager.ToggleBoxSprites(false);
        dotManager.ClearText();
        dotManager.ActivateTextField();
        ToggleIncorrectCodeText(false);
    }
}
