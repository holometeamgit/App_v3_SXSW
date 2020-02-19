using System.IO;
using UnityEngine;

public class JsonParser
{
    public static string ParseFileName(string code)
    {
        string path = HelperFunctions.PersistentDir() + code + (code.Contains(HelperFunctions.EXTJSON) ? string.Empty : HelperFunctions.EXTJSON);
        return File.ReadAllText(path);
    }

    public static T CreateFromJSON<T>(string jsonString) 
    {
        return JsonUtility.FromJson<T>(jsonString);
    }
}
