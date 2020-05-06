using System;
using System.IO;
using UnityEngine;

public class JsonParser
{
    public static string ParseFileName(string code)
    {
        string returnValue = "";

        try
        {

            string path = HelperFunctions.PersistentDir() + code + (code.Contains(HelperFunctions.EXTJSON) ? string.Empty : HelperFunctions.EXTJSON);
            returnValue = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Debug.LogError("Json parse error " + e);
        }

        return returnValue;
    }

    public static T CreateFromJSON<T>(string jsonString)
    {
        return JsonUtility.FromJson<T>(jsonString);
    }
}
