using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleHandler : MonoBehaviour
{
    protected StyleController styleController;
    protected MainDataContainer mainDataContainer;

    protected virtual void Awake() {
        Init();
    }

    protected void Init() {
        if (styleController == null)
            styleController = FindObjectOfType<StyleController>();

        if (mainDataContainer == null)
            mainDataContainer = FindObjectOfType<MainDataContainer>();
    }
}
