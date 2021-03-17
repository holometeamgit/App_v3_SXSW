using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchableToggle : Toggle
{
	protected override void Start() {
        base.Start();
        onValueChanged.AddListener(HideBackground);
        onValueChanged.Invoke(isOn);
    }

    private void HideBackground(bool value) {
        targetGraphic.gameObject.SetActive(!value);
    }

}
