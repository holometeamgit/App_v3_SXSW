using UnityEngine;

public class RaiseButtonFromScrollRect : MonoBehaviour {
    [SerializeField]
    private RectTransform contentContainer;

    [SerializeField]
    private RectTransform[] positions;

    private void OnEnable() {
        UpdatePosition();
    }

    /// <summary>
    /// Updated the position of the button based on the amount of active content items in the contentContainer gameobject
    /// </summary>
    public void UpdatePosition() {

        int activeChatMessages = 0;
        for (int i = 0; i < contentContainer.childCount; i++) {
            if (contentContainer.GetChild(i).gameObject.activeInHierarchy) {
                activeChatMessages++;
            }
        }
        transform.GetComponent<RectTransform>().sizeDelta = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].sizeDelta;
        transform.GetComponent<RectTransform>().anchoredPosition = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchoredPosition;
        transform.GetComponent<RectTransform>().anchorMin = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchorMin;
        transform.GetComponent<RectTransform>().anchorMax = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchorMax;
    }
}
