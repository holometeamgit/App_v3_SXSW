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
    IncorrectInputAnimationToggle incorrectInputAnimationToggle;

    private void Start()
    {
        dotManager.OnPassCodeEntered += TestCode;
    }

    private void OnEnable()
    {
        dotManager.ToggleBoxSprites(false);
        dotManager.ClearText();
    }

    void TestCode(string code)
    {
        if (code == "1337")
        {
            OnCodePassed?.Invoke();
        }
        else
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation();
        }
    }

}
