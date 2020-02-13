using System.IO;

public class JsonParser
{
    public static string ParseCode(string code)
    {
        string path = HelperFunctions.PersistentDir() + code + (code.Contains(HelperFunctions.EXTJSON) ? string.Empty : HelperFunctions.EXTJSON);
        return File.ReadAllText(path);
    }
}
