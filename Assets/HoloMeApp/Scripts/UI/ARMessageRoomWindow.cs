using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ARMessageRoomWindow
/// </summary>
public class ARMessageRoomWindow : MonoBehaviour {
    [SerializeField]
    private Animator _animator;

    /// <summary>
    /// Show btns
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
        _animator.SetBool("Show", true);
    }

    /// <summary>
    /// hide btns
    /// </summary>
    public void Hide() {
        _animator.SetBool("Show", false);
    }

    private void OnEnable() {
        Show();
    }

    private void OnDisable() {
        Hide();
    }
}
