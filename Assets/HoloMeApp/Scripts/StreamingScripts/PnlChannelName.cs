using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PnlChannelName : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputChannelName;

    [SerializeField]
    IncorrectInputAnimationToggle incorrectInputAnimationToggle;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    [SerializeField]
    AgoraController agoraController;

    private void Awake()
    {
        inputChannelName.characterLimit = 30;
    }

    public void OnReadyPressed()
    {
        //Any verification and validation should go here
        if (string.IsNullOrWhiteSpace(inputChannelName.text))
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation();
        }
        else
        {
            agoraController.ChannelName = inputChannelName.text.ToLower();
            OnChannelNamePassed?.Invoke();
        }
    }

    private void OnDisable()
    {
        inputChannelName.text = string.Empty;
    }
}
