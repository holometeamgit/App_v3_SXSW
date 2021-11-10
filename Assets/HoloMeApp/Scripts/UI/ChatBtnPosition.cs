using UnityEngine;

/// <summary>
/// This class is intended to raise button to specified positions in the positions array as items are added into a scroll rect in order to give a dynamic offset effect
/// </summary>
public class ChatBtnPosition : MonoBehaviour {
    [SerializeField]
    private RectTransform contentContainer;

    private const float SHIFT = 140;

    private const float MAX_COUNT = 6;

    private Vector3 basePosition;

    private void Awake() {
        basePosition = transform.localPosition;
    }

    private void OnEnable() {
        UpdatePosition();
    }

    /// <summary>
    /// Updated the position of the button based on the amount of active content items in the contentContainer gameobject
    /// </summary>
    public void UpdatePosition() {

        Vector3 position = transform.localPosition;

        position.y = basePosition.y + SHIFT * Mathf.Min(contentContainer.childCount, MAX_COUNT);

        transform.localPosition = position;

    }
}
