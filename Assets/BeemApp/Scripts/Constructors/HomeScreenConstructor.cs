using System;
using UnityEngine;
/// <summary>
/// Constructor for HomeScreen
/// </summary>
public class HomeScreenConstructor : MonoBehaviour {

    [Header("Bottom Menu Bar")]
    [SerializeField]
    protected GameObject _bottomMenuBar;

    [Header("Top Menu Bar")]
    [SerializeField]
    protected GameObject _topMenuBar;

    public static Action<bool> OnActivated = delegate { };

    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    protected void Activate(bool status) {
        _topMenuBar.SetActive(status);
        _bottomMenuBar.SetActive(status);
    }
}
