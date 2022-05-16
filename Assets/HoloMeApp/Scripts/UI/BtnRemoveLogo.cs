using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

/// <summary>
/// Call event for remove logo 
/// </summary>
public class BtnRemoveLogo : MonoBehaviour {
    public void RemoveLogo() {
        CallBacks.onRemoveLogo?.Invoke();
    }
}
