using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// ARHint
/// </summary>
public class ARHint {
    public enum State {
        Empty,
        Scan,
        TargetPlacement,
        HologramPlacement,
        ReadyToPinch
    };

    private State _currentState = State.Empty;

    public event Action onChangedState = delegate { };

    public State CurrentState {
        get {
            return _currentState;
        }
        set {
            _currentState = value;
            onChangedState?.Invoke();
        }
    }

    public ARHint() {

    }

    ~ARHint() {

    }

}
