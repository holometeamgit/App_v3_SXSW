using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class KeyBoardPositionController : MonoBehaviour {
    private RectTransform rectTransform;

    private int height;

    private void OnEnable() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        if (height == KeyBoardData.GetKeyboardHeight(false)) {
            return;
        }
        UpdatePosition();
    }

    public void UpdatePosition() {
        height = KeyBoardData.GetKeyboardHeight(false);
        Vector2 vector2 = rectTransform.anchoredPosition;
        vector2.y = height;
        rectTransform.anchoredPosition = vector2;
        Debug.Log(rectTransform.anchoredPosition);
    }
}
