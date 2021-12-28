using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIBtnRoomAndMsg controller for switch btn
/// </summary>
public class UIBtnRoomAndMsg : MonoBehaviour {
    [SerializeField]
    private Animator _animator;

    /// <summary>
    /// Show btns
    /// </summary>
    public void Show() {
        ARMessageRoomConstructor.OnActivated?.Invoke(true);
        _animator.SetBool("Show", true);
    }

    /// <summary>
    /// hide btns
    /// </summary>
    public void Hide() {
        ARMessageRoomConstructor.OnActivated?.Invoke(false);
        _animator.SetBool("Show", false);
    }

    /// <summary>
    /// switch value 
    /// </summary>
    public void SwitchValue() {
        ARMessageRoomConstructor.OnActivated?.Invoke(!_animator.GetBool("Show"));
        _animator.SetBool("Show", !_animator.GetBool("Show"));
    }

    private void OnDisable() {
        Hide();
    }
}
