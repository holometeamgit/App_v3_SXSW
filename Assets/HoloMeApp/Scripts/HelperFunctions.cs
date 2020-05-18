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
}