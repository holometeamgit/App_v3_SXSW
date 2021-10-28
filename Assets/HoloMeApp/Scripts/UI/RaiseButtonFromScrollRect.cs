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

        RectTransform rectReference = transform.GetComponent<RectTransform>();
        RectTransform rectToMoveTo = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1];

        rectReference.sizeDelta = rectToMoveTo.sizeDelta;
        rectReference.anchoredPosition = rectToMoveTo.anchoredPosition;
        rectReference.anchorMin = rectToMoveTo.anchorMin;
        rectReference.anchorMax = rectToMoveTo.anchorMax;
    }
}
