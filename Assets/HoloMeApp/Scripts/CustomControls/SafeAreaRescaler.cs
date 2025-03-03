﻿using UnityEngine;

/// <summary>
/// Use this to keep UI elements within bounds of the screen safe area
/// </summary>
public class SafeAreaRescaler : MonoBehaviour {

    [Tooltip("Check this to ignore bottom of screen")]
    [SerializeField]
    private bool ignoreLowerScreen = true;

    private RectTransform Panel;
    private Rect LastSafeArea = new Rect(0, 0, 0, 0);

    private void Awake() {
        Panel = GetComponent<RectTransform>();
        Refresh();
    }
    private void Update() {
        Refresh();
    }
    private void Refresh() {
        Rect safeArea = GetSafeArea();
        if (ignoreLowerScreen) {
            safeArea = new Rect(safeArea.x, 0, safeArea.width, safeArea.height + safeArea.y);
        }
        if (safeArea != LastSafeArea) {
            ApplySafeArea(safeArea);
        }
    }
    private Rect GetSafeArea() {
        return Screen.safeArea;
    }
    private void ApplySafeArea(Rect r) {
        LastSafeArea = r;
        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
        //Debug.LogFormat(“New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}“,
        //    name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
    }
}