using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Alpha Btn
/// </summary>
public class StateAlphaBtn : MonoBehaviour {
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private bool _startState = true;

    private bool _currentState = default;

    private void OnEnable() {
        _currentState = _startState;
        UpdateView();
    }

    /// <summary>
    /// Update State
    /// </summary>
    public void ChangeState() {
        _currentState = !_currentState;
        UpdateView();
    }

    private void UpdateView() {
        _canvasGroup.alpha = _currentState ? 1 : 0;
    }
}
