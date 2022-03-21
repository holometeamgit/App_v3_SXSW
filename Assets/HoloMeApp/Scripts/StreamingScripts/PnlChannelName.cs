using UnityEngine;
using UnityEngine.Events;
using Crosstales.BWF;
using Zenject;

public class PnlChannelName : MonoBehaviour {
    [SerializeField]
    InputFieldController inputFieldController;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AgoraRequests agoraRequests;

    [SerializeField]
    GameObject btnContinue;

    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    private void Awake() {
        inputFieldController.characterLimit = HelperFunctions.ChannelNameCharacterLimit;
        //requestChannelList = new RequestChannelList();
        //requestChannelList.OnSuccessAction += OnChannelListOccupied;
    }

    public void ConfirmFilmingGuidelines() {
        PlayerPrefs.SetInt("ConfirmFilmingGuidelines", 1);
        CheckConfirmFilmingGuidelines();
    }

    public bool IsConfirmFilmingGuidelines() {
        return PlayerPrefs.HasKey("ConfirmFilmingGuidelines") && PlayerPrefs.GetInt("ConfirmFilmingGuidelines") == 1;
    }

    public void OnReadyPressed() {
        //Need to disable button interactability here while waiting for callback

        if (string.IsNullOrWhiteSpace(inputFieldController.text) || BWFManager.Contains(inputFieldController.text, Crosstales.BWF.Model.ManagerMask.BadWord)) {
            inputFieldController.ShowWarning("Please Enter A Valid Name");
        } else {
            OnChannelListOccupied();
            //agoraRequests.MakeGetRequest(requestChannelList);
        }
    }

    void OnChannelListOccupied() {
        //bool doesChannelExist = requestChannelList.DoesChannelExist(inputFieldController.text);

        //if (doesChannelExist)
        //{
        //    inputFieldController.ShowWarning("Channel Already Exists!");
        //}
        //else
        //{
        //agoraController.ChannelName = inputFieldController.text.ToLower();
        OnChannelNamePassed?.Invoke();
        //}
    }

    private void CheckConfirmFilmingGuidelines() {
        btnContinue.SetActive(IsConfirmFilmingGuidelines());
    }

    private void OnEnable() {
        CheckConfirmFilmingGuidelines();
        inputFieldController.text = _userWebManager.GetUsername();
        agoraController.ChannelName = inputFieldController.text;
        inputFieldController.interactable = false;
    }

    private void OnDisable() {
        inputFieldController.text = string.Empty;
    }
}
