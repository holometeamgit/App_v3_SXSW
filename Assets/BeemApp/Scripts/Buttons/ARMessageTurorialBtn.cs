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
        SettingsConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(false);
        HomeScreenConstructor.OnActivated?.Invoke(false);
        StreamCallBacks.onCloseComments?.Invoke();
        ARMessageTutorialConstructor.OnActivated?.Invoke(true);
    }
}
