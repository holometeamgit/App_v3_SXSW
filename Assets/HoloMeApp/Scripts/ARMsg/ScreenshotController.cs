using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.ARMsg;

/// <summary>
/// ScreenshotController. Can make screenshot 
/// </summary>
public class ScreenshotController : MonoBehaviour {
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

    private IEnumerator AddingToImage() {
        int width;
        int heigh;
        AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out width, out heigh);

        HelperFunctions.DevLog("screenshot width " + width + " heigh " + heigh);

        yield return new WaitForEndOfFrame();

        string path = GetPathToFile();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        yield return new WaitForEndOfFrame();

        TextureScale.Scale(screenImage, width, heigh);
        yield return new WaitForEndOfFrame();
        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();

        //Save image to file
        System.IO.File.WriteAllBytes(path, imageBytes);
        yield return new WaitForEndOfFrame();
    }

    private void OnDestroy() {
        CallBacks.OnScreenshotRequest -= MakeScreenShotNow;
        CallBacks.OnGetBGImgFilePath -= GetPathToFile;
    }
}
