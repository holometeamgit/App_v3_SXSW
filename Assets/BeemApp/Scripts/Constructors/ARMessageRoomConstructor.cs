using System;
using UnityEngine;

/// <summary>
/// Constructor for ARMessageRoom Popup
/// </summary>
public class ARMessageRoomConstructor : MonoBehaviour {

    [SerializeField]
    private ARMessageRoomWindow _arMessageRoomWindow;

    public static Action<bool> OnActivated = delegate { };

    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    protected void Activate(bool status) {
        if (status) {
            _arMessageRoomWindow.Show();
        } else {
            _arMessageRoomWindow.Hide();
        }
    }
}
