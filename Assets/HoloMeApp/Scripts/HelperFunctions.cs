using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelperFunctions {
    public const string EXTJSON = ".json";
    public const string EXTMP4 = ".mp4";
    public const string EXTPNG = ".png";

    public const string versionFile = "Version.json";

    public const int ChannelNameCharacterLimit = 30;

    private const string TAG = "#Tag";

    public static bool IsVideoThumbnailData(string name) {
        return name.Contains("VidThumb");
    }

    public static string PersistentDir() {
        return Application.persistentDataPath + "/";
    }

    public static T GetTypeIfNull<T>(MonoBehaviour behaviour) where T : MonoBehaviour {
        if (behaviour == null) {
            if (Application.isEditor) //In the editor try getting a list of this type to check for duplicated via log message
            {
                var components = GameObject.FindObjectsOfType<T>();

                if (components != null && components.Length > 0) {
                    if (components.Length > 1) {
                        DevLogError($"More than one of type {components[0].GetType()} was found, this function should only find 1 monobehaviour in the scene");
                    }
                    if (components[0] == null) {
                        Debug.LogError($"No component of type {behaviour} was found in the scene");
                    } else {
                        return components[0];
                    }
                } else if (components == null || components.Length <= 0) {
                    Debug.LogError($"No component of type {behaviour} was found in the scene");
                }
            } else {
                if (behaviour == null) {
                    var returnComponent = GameObject.FindObjectOfType<T>();
                    if (returnComponent == null) {
                        Debug.LogError($"No component of type {behaviour} was found in the scene");
                    }

                    return returnComponent;
                }
            }
        }

        return behaviour as T;
    }

    public static string GetExtension(string filename) {
        if (filename.Contains(EXTJSON)) {
            return EXTJSON;
        } else if (filename.Contains(EXTPNG)) {
            return EXTPNG;
        } else if (filename.Contains(EXTMP4)) {
            return EXTMP4;
        } else {
            //Debug.LogError("Extension Not Found!");
            return "";
        }
    }

    public static bool IsFileJSON(string filename) {
        if (File.Exists(PersistentDir() + filename + EXTJSON)) {
            return true;
        } else return false;
    }

    public static bool AllJSONFilesLocallyAvailable(string jsonCode) {
        if (IsFileJSON(jsonCode)) {
            var videoJsonData = JsonParser.CreateFromJSON<VideoJsonData>(JsonParser.ParseFileName(jsonCode));
            return (DoesFileExist(videoJsonData.logoImage) && DoesFileExist(videoJsonData.videoCode));
        } else {
            Debug.LogError($"{nameof(AllJSONFilesLocallyAvailable)} File was not of JSON type {jsonCode}");
            return false;
        }
    }

    //TODO: Update this to iterate through the all files with filename and check it's extension maybe?
    public static bool DoesFileExist(string filename) {
        if (File.Exists(PersistentDir() + filename)) //Case if the filename string already contains the extension
        {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTMP4)) {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTJSON)) {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTPNG)) {
            return true;
        }

        //Debug.LogError(nameof(DoesFileExist) + "Extension Not Found! Name = " + filename);
        return false;
    }

    public static Color GetColor(int r, int g, int b, int a = 1) {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <summary>
    /// Use this to detect if UI is being pressed
    /// </summary>
    public static bool IsPointerOverUIObject() {
        if (Input.touchCount > 0) {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.GetTouch(0).position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        return false;
    }

    /// <summary>
    /// Log message only in editor or debug builds
    /// </summary>
    public static void DevLog(string message, string tag = "") {
        if (Application.isEditor || Debug.isDebugBuild) {
            if (string.IsNullOrEmpty(tag)) {
                Debug.Log(message);
            } else {
                Debug.Log(message + TAG + tag);
            }
        }
    }

    /// <summary>
    /// Log warnings only in editor or debug builds
    /// </summary>
    public static void DevLogWarning(string message, string tag = "") {
        if (Application.isEditor || Debug.isDebugBuild) {
            if (string.IsNullOrEmpty(tag)) {
                Debug.LogWarning(message);
            } else {
                Debug.LogWarning(message + TAG + tag);
            }
        }
    }

    /// <summary>
    /// Log errors only in editor or debug builds
    /// </summary>
    public static void DevLogError(string message, string tag = "") {
        if (Application.isEditor || Debug.isDebugBuild) {
            if (string.IsNullOrEmpty(tag)) {
                Debug.LogError(message);
            } else {
                Debug.LogError(message + TAG + tag);
            }
        }
    }
}