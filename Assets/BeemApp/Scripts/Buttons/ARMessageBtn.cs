using Beem.ARMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Btn for opening armessage
/// </summary>
public class ARMessageBtn : MonoBehaviour {

    /// <summary>
    /// Open Btn
    /// </summary>
    public void Open() {
        ARMessageTutorialConstructor.OnHide?.Invoke();
        ARMsgRecordConstructor.OnShow?.Invoke();
    }
}
