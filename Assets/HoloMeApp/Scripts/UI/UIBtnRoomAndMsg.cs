using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIBtnRoomAndMsg controller for switch btn
/// </summary>
public class UIBtnRoomAndMsg : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    /// <summary>
    /// Show btns
    /// </summary>
    public void Show() {
        _animator.SetBool("Show", true);
    }

    /// <summary>
    /// hide btns
    /// </summary>
    public void Hide() {
        _animator.SetBool("Show", false);
    }

    /// <summary>
    /// switch value 
    /// </summary>
    public void SwitchValue() {
        _animator.SetBool("Show", !_animator.GetBool("Show"));
    }

    private void OnDisable() {
        Hide();
    }
}
