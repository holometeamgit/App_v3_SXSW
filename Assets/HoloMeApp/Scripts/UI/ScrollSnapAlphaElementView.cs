using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change Alpha in View
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ScrollSnapAlphaElementView : MonoBehaviour, IScrollSnapElementView {

    [SerializeField]
    private Vector2 limit;

    public Vector2 Limit => limit;


    private CanvasGroup canvasGroup;

    /// <summary>
    /// Change Alpha
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <param name="value"></param>
    /// <param name="maxNumber"></param>
    public void OnProgress(float elementNumber, float value, float maxNumber) {
        if (canvasGroup == null) {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        float difference = 1 - Mathf.Abs((elementNumber - value) / maxNumber);
        canvasGroup.alpha = limit.x + difference * (limit.y - limit.x);
    }
}
