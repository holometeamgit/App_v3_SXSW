using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Crosstales.BWF;

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

    [SerializeField]
    AgoraRequests agoraRequests;

    RequestChannelList requestChannelList;

    private void Awake()
    {
        inputChannelName.characterLimit = HelperFunctions.ChannelNameCharacterLimit;
        requestChannelList = new RequestChannelList();
        requestChannelList.OnSuccessAction += OnChannelListOccupied;
    }

    public void OnReadyPressed()
    {
        //Need to disable button interactability here while waiting for callback

        if (string.IsNullOrWhiteSpace(inputChannelName.text) && BWFManager.Contains(inputChannelName.text, Crosstales.BWF.Model.ManagerMask.BadWord))
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation(incorrectMessage: "Please Enter A Valid Name");
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
            incorrectInputAnimationToggle.StartIncorrectAnimation(incorrectMessage: "Channel Already Exists!");
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
