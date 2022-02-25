using Beem.Permissions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage tutorial
/// </summary>
public class ARMessageTurorialBtn : MonoBehaviour {

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        MenuConstructor.OnActivated?.Invoke(false);
        ARMessageTutorialConstructor.OnActivated?.Invoke(true);
    }
}
