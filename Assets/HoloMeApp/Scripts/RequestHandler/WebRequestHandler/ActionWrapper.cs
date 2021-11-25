using System;
/// <summary>
/// The class wraps Acttion. Required to pass an instance of the class as an argument. It also remembers if the event has already happened.
/// </summary>
public class ActionWrapper {

    private Action onAction;
    private bool _wasCalled;

    public bool WasCalled {
        get { return _wasCalled; }
    }

    /// <summary>
    /// AddListener
    /// </summary>
    public void AddListener(Action onAction) {
        this.onAction += onAction;
    }

    /// <summary>
    /// RemoveListener
    /// </summary>
    public void RemoveListener(Action onAction) {
        this.onAction -= onAction;
    }

    /// <summary>
    /// RemoveAll
    /// </summary>
    public void RemoveAll() {
        this.onAction = null;
    }

    public void InvokeAction() {
        _wasCalled = true;
        onAction?.Invoke();
    }
}
