using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract Settings for Keyboard
/// </summary>
public abstract class AbstractKeyBoardSettings : MonoBehaviour {

    /// <summary>
    /// Refresh keyboard data
    /// </summary>
    /// <param name="inputField"></param>
    public abstract void RefreshData(InputField inputField);
}
