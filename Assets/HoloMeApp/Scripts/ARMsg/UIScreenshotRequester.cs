using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.ARMsg;

public class UIScreenshotRequester : MonoBehaviour
{
    public void MakeScreenShot() {
        CallBacks.OnScreenshotRequest?.Invoke();
    }
}
