using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeechNotificationPopups : MonoBehaviour {
    [SerializeField]
    private Dictionary<string, GameObject> visiblePopups = new Dictionary<string, GameObject>();

    [SerializeField]
    Stack<GameObject> disabledPopups = new Stack<GameObject>();

    [SerializeField]
    Transform controlsParent;

    [SerializeField]
    GameObject imgSpeakingRectablePrefabReferece;

    const int MAX_CHAT_BOX_COUNT = 10;

    /// <summary>
    /// Activate popup for user with name
    /// </summary>
    public void ActivatePopup(string name) {
        print("Activate POPUP CALLED " + name);
        if (visiblePopups.Count >= MAX_CHAT_BOX_COUNT)
            return;

        if (visiblePopups.ContainsKey(name))
            return;

        GameObject newImagePopUp = GetPopup(name);
        visiblePopups.Add(name, newImagePopUp);
    }

    private GameObject GetPopup(string name) {
        GameObject popupToReturn;

        if (disabledPopups.Count > 0) {
            popupToReturn = disabledPopups.Pop();
            popupToReturn.gameObject.SetActive(true);
        } else {
            popupToReturn = Instantiate(imgSpeakingRectablePrefabReferece, controlsParent);
        }
        popupToReturn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        return popupToReturn;
    }

    /// <summary>
    /// Disables popup for user with name
    /// </summary>
    public void DeactivatePopup(string name) {
        print("Deactivate POPUP CALLED " + name);
        if (visiblePopups.ContainsKey(name)) {
            GameObject popupToDisable = visiblePopups[name];
            popupToDisable.SetActive(false);
            disabledPopups.Push(popupToDisable);
            visiblePopups.Remove(name);
        }
    }

    /// <summary>
    /// Disable all visible popups
    /// </summary>
    public void DeactivateAllPopups() {
        foreach (KeyValuePair<string, GameObject> popup in visiblePopups) {
            DeactivatePopup(popup.Key);
        }
    }
}
