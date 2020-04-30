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

    public void OnReadyPressed()
    {
        //Any verification and validation should go here
        if (string.IsNullOrWhiteSpace(inputChannelName.text))
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation();
        }
        else
        {
            agoraController.ChannelName = inputChannelName.text;
            OnChannelNamePassed?.Invoke();
        }
    }

}
