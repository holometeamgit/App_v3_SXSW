using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PnlWatchLive : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputChannelName;

    [SerializeField]
    IncorrectInputAnimationToggle incorrectInputAnimationToggle;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AgoraRequests agoraRequests;

    RequestChannelList requestChannelList;

    private void Awake()
    {
        inputChannelName.characterLimit = HelperFunctions.ChannelNameCharacterLimit;
        requestChannelList = new RequestChannelList();
        requestChannelList.OnSuccessAction -= OnChannelListOccupied;
        requestChannelList.OnSuccessAction += OnChannelListOccupied;
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
            agoraRequests.RequestChannels(requestChannelList);
        }
    }

    void OnChannelListOccupied()
    {
        bool doesChannelExist = requestChannelList.DoesChannelExist(inputChannelName.text);

        if (doesChannelExist)
        {
            agoraController.ChannelName = inputChannelName.text.ToLower();
            OnChannelNamePassed?.Invoke();
        }
        else
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation(incorrectMessage: "Channel Doesn't Exist!");
        }
    }

    private void OnDisable()
    {
        inputChannelName.text = string.Empty;
    }
}
