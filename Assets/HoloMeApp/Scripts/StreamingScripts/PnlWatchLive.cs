using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class PnlWatchLive : MonoBehaviour {
    [SerializeField]
    InputFieldController inputFieldController;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    [SerializeField]
    AgoraRequests agoraRequests;

    RequestChannelList requestChannelList;
    private AgoraController _agoraController;

    [Inject]
    public void Construct(AgoraController agoraController) {
        _agoraController = agoraController;
    }

    private void Awake() {
        inputFieldController.characterLimit = HelperFunctions.ChannelNameCharacterLimit;
        requestChannelList = new RequestChannelList();
        requestChannelList.OnSuccessAction -= OnChannelListOccupied;
        requestChannelList.OnSuccessAction += OnChannelListOccupied;
    }

    public void OnReadyPressed() {
        //Any verification and validation should go here
        if (string.IsNullOrWhiteSpace(inputFieldController.text)) {
            inputFieldController.ShowWarning("Please enter a valid name");
        } else {
            agoraRequests.MakeGetRequest(requestChannelList);
        }
    }

    void OnChannelListOccupied() {
        bool doesChannelExist = requestChannelList.DoesChannelExist(inputFieldController.text);

        if (doesChannelExist) {
            _agoraController.ChannelName = inputFieldController.text.ToLower();
            OnChannelNamePassed?.Invoke();
        } else {
            inputFieldController.ShowWarning("Channel Doesn't Exist!");
        }
    }

    private void OnDisable() {
        inputFieldController.text = string.Empty;
    }
}
