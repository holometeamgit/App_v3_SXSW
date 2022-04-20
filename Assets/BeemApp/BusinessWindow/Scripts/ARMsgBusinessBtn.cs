using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Btn
/// </summary>

public class ARMsgBusinessBtn : MonoBehaviour, IARMsgDataView {

    private ARMsgJSON.Data _data;

    public void Init(ARMsgJSON.Data data) {
        _data = data;
    }

    public void OnClick() {
        BusinessOptionsConstructor.OnShow?.Invoke(_data, false);
    }
}
