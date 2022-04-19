using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Back Business Options Btn
/// </summary>
public class BackBusinessOptionsBtn : MonoBehaviour {
    /// <summary>
    /// Close Business Options
    /// </summary>
    public void Close() {
        BusinessOptionsConstructor.OnShow?.Invoke(null);
    }
}
