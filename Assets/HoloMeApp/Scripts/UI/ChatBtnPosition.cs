using UnityEngine;

/// <summary>
/// This class is intended to raise button to specified positions in the positions array as items are added into a scroll rect in order to give a dynamic offset effect
/// </summary>
public class ChatBtnPosition : MonoBehaviour {
    [SerializeField]
    private RectTransform contentContainer;

    [SerializeField]
    private RectTransform viewport;

    private RectTransform buttonsRectTrans;

    private const float OFFSET = 50;

    private float contentContaineSizeRef;

    private bool limitReached;

    private void Awake() {
        buttonsRectTrans = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        contentContaineSizeRef = -1;
        limitReached = false;
    }

    private void Update() {
        UpdatePosition();
    }

    /// <summary>
    /// Updated the position of the button based on the amount of active content items in the contentContainer gameobject
    /// </summary>
    public void UpdatePosition() {

        if (limitReached) {
            return;
        }

        if (contentContainer.rect.size.y <= viewport.rect.size.y) {
            if (contentContaineSizeRef != contentContainer.rect.size.y) {
                buttonsRectTrans.anchoredPosition = new Vector2(buttonsRectTrans.anchoredPosition.x, contentContainer.rect.size.y + OFFSET);
                contentContaineSizeRef = contentContainer.rect.size.y;

            }
        } else {
            buttonsRectTrans.anchoredPosition = new Vector2(buttonsRectTrans.anchoredPosition.x, viewport.rect.size.y + OFFSET);
            limitReached = true;
        }
    }
}
