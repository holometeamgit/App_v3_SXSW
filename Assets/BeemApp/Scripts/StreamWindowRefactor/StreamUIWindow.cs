using Beem.UI;
using UnityEngine;
using UnityEngine.UI;

public class StreamUIWindow : MonoBehaviour {
    [SerializeField]
    private GameObject controlsPresenter;

    [SerializeField]
    private GameObject controlsViewer;

    [SerializeField]
    private GameObject[] publicStreamsControls; //TODO rename to stadium

    [SerializeField]
    private GameObject[] privateStreamsControls; //TODO rename to room

    [SerializeField]
    private GameObject[] onlineControls;

    [SerializeField]
    private GameObject[] offlineControls;

    [Tooltip("Controls which interactable bool will be set to true when live")]
    [SerializeField]
    private Selectable[] onlineInteractableControlToggle;

    [SerializeField]
    private Toggle togglePushToTalk;

    /// <summary>
    /// Toggle push to talk interactability
    /// </summary>
    public Toggle TogglePustoTalk { get => togglePushToTalk; set => togglePushToTalk = value; }

    [SerializeField]
    private GameObject[] gameObjectsToDisableWhileGoingLive;

    [SerializeField]
    private UIBtnLikes uiBtnLikes;

    [SerializeReference]
    private UITextLabelLikes uiViewersTextLabelLikes;

    [SerializeField]
    private SpeechNotificationPopups speechNotificationPopups;

    public SpeechNotificationPopups SpeechNotificationPopups { get => speechNotificationPopups; }

    /// <summary>
    /// Initialises some UI controls
    /// </summary>
    /// <param name="streamID"></param>
    public void InitLikes(long streamID) {
        uiBtnLikes.Init(streamID);
        uiViewersTextLabelLikes.Init(streamID);
    }

    /// <summary>
    /// Activates the relevant gameobject for room or stadium
    /// </summary>
    public void RefreshStreamControls(bool room) {
        foreach (GameObject item in privateStreamsControls) {
            item.SetActive(room);
        }
        foreach (GameObject item in publicStreamsControls) {
            item.SetActive(!room);
        }
    }

    /// <summary>
    /// Toggles controls if live or not
    /// </summary>
    public void TogglePreLiveControls(bool enable) {
        foreach (GameObject objectToToggle in gameObjectsToDisableWhileGoingLive) {
            objectToToggle.SetActive(enable);
        }
    }

    /// <summary>
    /// Toggles correct game object based on viewer or broadcaster
    /// </summary>
    public void RefreshBroadcasterControls(bool broadcaster) {
        controlsPresenter.SetActive(broadcaster);
        controlsViewer.SetActive(!broadcaster);
    }

    public void RefreshLiveControls(bool live) {

        foreach (GameObject item in onlineControls) {
            item.SetActive(live);
        }
        foreach (Selectable selectable in onlineInteractableControlToggle) {
            selectable.interactable = live;
        }
        foreach (GameObject item in offlineControls) {
            item.SetActive(!live);
        }
    }

}
