using System;

public class ActionWrapper {

    private Action onAction;
    private bool _wasCalled;

    public bool WasCalled {
        get { return _wasCalled; }
    }

    public void AddListener(Action onAction) {
        this.onAction += onAction;
    }

    public void RemoveListener(Action onAction) {
        this.onAction -= onAction;
    }

    public void RemoveAll() {
        this.onAction = null;
    }

    public void InvokeAction() {
        _wasCalled = true;
        onAction?.Invoke();
    }
}
