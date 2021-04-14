using System.IO;
using UnityEngine;

public class HelperFunctions
{
    public const string EXTJSON = ".json";
    public const string EXTMP4 = ".mp4";
    public const string EXTPNG = ".png";

    public const string versionFile = "Version.json";

    public const int ChannelNameCharacterLimit = 30;

    public static bool IsVideoThumbnailData(string name)
    {
        return name.Contains("VidThumb");
    }

    public static string PersistentDir()
    {
        return Application.persistentDataPath + "/";
    }

    public static T GetTypeIfNull<T>(MonoBehaviour behaviour) where T : MonoBehaviour
    {
        if (behaviour == null)
        {
            if (Application.isEditor) //In the editor try getting a list of this type to check for duplicated via log message
            {
                var components = GameObject.FindObjectsOfType<T>();

                if (components != null && components.Length > 0)
                {
                    if (components.Length > 1)
                    {
                        DevLogError($"More than one of type {components[0].GetType()} was found, this function should only find 1 monobehaviour in the scene");
                    }
                    if (components[0] == null)
                    {
                        Debug.LogError($"No component of type {behaviour} was found in the scene");
                    }
                    else
                    {
                        return components[0];
                    }
                }
                else if (components == null || components.Length <= 0)
                {
                    Debug.LogError($"No component of type {behaviour} was found in the scene");
                }
            }
            else
            {
                if (behaviour == null)
                {
                    var returnComponent = GameObject.FindObjectOfType<T>();
                    if (returnComponent == null)
                    {
                        Debug.LogError($"No component of type {behaviour} was found in the scene");
                    }

                    return returnComponent;
                }
            }
        }

        return behaviour as T;
    }

    public static string GetExtension(string filename)
    {
        if (filename.Contains(EXTJSON))
        {
            return EXTJSON;
        }
        else if (filename.Contains(EXTPNG))
        {
            return EXTPNG;
        }
        else if (filename.Contains(EXTMP4))
        {
            return EXTMP4;
        }
        else
        {
            //Debug.LogError("Extension Not Found!");
            return "";
        }
    }

    public static bool IsFileJSON(string filename)
    {
        if (File.Exists(PersistentDir() + filename + EXTJSON))
        {
            return true;
        }
        else return false;
    }

    public static bool AllJSONFilesLocallyAvailable(string jsonCode)
    {
        if (IsFileJSON(jsonCode))
        {
            var videoJsonData = JsonParser.CreateFromJSON<VideoJsonData>(JsonParser.ParseFileName(jsonCode));
            return (DoesFileExist(videoJsonData.logoImage) && DoesFileExist(videoJsonData.videoCode));
        }
        else
        {
            Debug.LogError($"{nameof(AllJSONFilesLocallyAvailable)} File was not of JSON type {jsonCode}");
            return false;
        }
    }

    //TODO: Update this to iterate through the all files with filename and check it's extension maybe?
    public static bool DoesFileExist(string filename)
    {
        if (File.Exists(PersistentDir() + filename)) //Case if the filename string already contains the extension
        {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTMP4))
        {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTJSON))
        {
            return true;
        }

        if (File.Exists(PersistentDir() + filename + EXTPNG))
        {
            return true;
        }

        //Debug.LogError(nameof(DoesFileExist) + "Extension Not Found! Name = " + filename);
        return false;
    }

    public static Color GetColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static void DevLog(string message)
    {
        if (Application.isEditor || Debug.isDebugBuild)
            Debug.Log(message);
    }

    public static void DevLogError(string message)
    {
        if (Application.isEditor || Debug.isDebugBuild)
            Debug.LogError(message);
    }
}