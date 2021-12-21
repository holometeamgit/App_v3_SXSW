using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scroll Element View
/// </summary>
public interface IScrollSnapElementView {
    Vector2 Limit { get; }
    void OnProgress(float elementNumber, float value, float maxNumber);
}
