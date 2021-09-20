using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Dynamic sensitivity adjustment for different screens (different dpi)
/// </summary>

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
