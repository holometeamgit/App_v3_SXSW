using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change Pivot in View
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ScrollSnapPivotElement : MonoBehaviour, IScrollSnapElementView {

    [SerializeField]
    private Vector2 limit;
    [SerializeField]
    private Vector2 basePivot = new Vector2(0.5f, 0.5f);

    public Vector2 Limit => limit;
    private RectTransform rectTranform;

    /// <summary>
    /// Change Pivot
    /// </summary>

    public void OnProgress(float elementNumber, float value, float maxNumber) {
        if (rectTranform == null) {
            rectTranform = GetComponent<RectTransform>();
        }

        float pivot = value - elementNumber;

        rectTranform.pivot = basePivot + Vector2.right * (limit.x + (pivot) * (limit.y - limit.x));
    }
}
