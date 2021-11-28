using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.ARMsg;

public class ScreenshotController : MonoBehaviour
{
    private const string FILE_NAME = "Screenshot.png";

    private void Start() {
        CallBacks.OnScreenshotRequest += MakeScreenShotNow;
        CallBacks.OnGetBGImgFilePath += GetPathToFile;
    }

    private void MakeScreenShotNow() {
        StartCoroutine(AddingToImage());
    }

    private string GetPathToFile() {
#if !UNITY_EDITOR
        return System.IO.Path.Combine(Application.persistentDataPath, FILE_NAME);// Application.persistentDataPath + "/" + FILE_NAME;
#else
        return Application.dataPath + "/../" + FILE_NAME;
#endif

    }

    IEnumerator AddingToImage() {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(FILE_NAME);
        yield return new WaitForEndOfFrame();
    }

    private void OnDestroy() {
        CallBacks.OnScreenshotRequest -= MakeScreenShotNow;
        CallBacks.OnGetBGImgFilePath -= GetPathToFile;
    }
}
