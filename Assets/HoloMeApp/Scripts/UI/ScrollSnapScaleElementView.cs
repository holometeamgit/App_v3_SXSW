using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change Scale in View
/// </summary>
public class ScrollSnapScaleElementView : MonoBehaviour, IScrollSnapElementView {

    [SerializeField]
    private Vector2 limit;

    public Vector2 Limit => limit;

    /// <summary>
    /// Change Scale
    /// </summary>
    /// <param name="elementNumber"></param>
    /// <param name="value"></param>
    /// <param name="maxNumber"></param>
    public void OnProgress(float elementNumber, float value, float maxNumber) {
        float difference = 1 - Mathf.Abs(elementNumber - value);
        transform.localScale = Vector3.one * (limit.x + difference * (limit.y - limit.x));
    }
}
