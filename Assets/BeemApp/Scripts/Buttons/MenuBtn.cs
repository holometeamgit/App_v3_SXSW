using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Menu Btn
/// </summary>
public class MenuBtn : MonoBehaviour {
    [SerializeField]
    private bool isOpened;
    public void OnClick() {
        MenuConstructor.OnActivated?.Invoke(isOpened);
    }
}
