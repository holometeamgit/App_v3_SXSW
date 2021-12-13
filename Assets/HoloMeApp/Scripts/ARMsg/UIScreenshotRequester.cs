using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

/// <summary>
/// UIScreenshotRequester. Request make a screenshot
/// </summary>
public class UIScreenshotRequester : MonoBehaviour
{
    /// <summary>
    /// MakeScreenShot
    /// </summary>
    public void MakeScreenShot() {
        CallBacks.OnScreenshotRequest?.Invoke();
    }
}
