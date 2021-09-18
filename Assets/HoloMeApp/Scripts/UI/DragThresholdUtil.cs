using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragThresholdUtil : MonoBehaviour {
    private const float MIDDLE_VALUE_DPI = 160f;

    void Start() {
        int defaultValue = EventSystem.current.pixelDragThreshold;
        EventSystem.current.pixelDragThreshold =
                Mathf.Max(
                     defaultValue,
                     (int)(defaultValue * Screen.dpi / MIDDLE_VALUE_DPI));
    }
}