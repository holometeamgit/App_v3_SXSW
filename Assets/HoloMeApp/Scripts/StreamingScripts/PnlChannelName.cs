using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Crosstales.BWF;

public class PnlChannelName : MonoBehaviour
{
    [SerializeField]
    InputFieldController inputFieldController;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AgoraRequests agoraRequests;

    RequestChannelList requestChannelList;

    private void Awake()
    {
        inputFieldController.characterLimit = HelperFunctions.ChannelNameCharacterLimit;
        requestChannelList = new RequestChannelList();
        requestChannelList.OnSuccessAction += OnChannelListOccupied;
    }

    public void OnReadyPressed()
    {
        //Need to disable button interactability here while waiting for callback

        if (string.IsNullOrWhiteSpace(inputFieldController.text) && BWFManager.Contains(inputFieldController.text, Crosstales.BWF.Model.ManagerMask.BadWord))
        {
            inputFieldController.ShowWarning("Please Enter A Valid Name");
        }
        else
        {
            agoraRequests.RequestChannels(requestChannelList);
        }
    }

    void OnChannelListOccupied()
    {
        bool doesChannelExist = requestChannelList.DoesChannelExist(inputFieldController.text);

        if (doesChannelExist)
        {
            inputFieldController.ShowWarning("Channel Already Exists!");
        }
        else
        {
            agoraController.ChannelName = inputFieldController.text.ToLower();
            OnChannelNamePassed?.Invoke();
        }
    }

    private void OnDisable()
    {
        inputFieldController.text = string.Empty;
    }
}
