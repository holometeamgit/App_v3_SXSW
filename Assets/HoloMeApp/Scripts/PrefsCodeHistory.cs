using System.Collections.Generic;
using UnityEngine;

public static class PrefsCodeHistory
{
    const string PrefCodeMemory = nameof(PrefCodeMemory);

    const int CodeSaveLimit = 10;

    public static string[] GetSavedCodes()
    {
        string codeString = PlayerPrefs.GetString(PrefCodeMemory, "").ToLower();
        if (codeString == "")
        {
            return new string[] { };
        }
        return codeString.Split(' ');
    }

    public static void RemoveCode(string code)
    {
        string codeString = PlayerPrefs.GetString(PrefCodeMemory, "");

        if (!codeString.Contains(code))
            return;

        List<string> codeStringSplit = new List<string>(codeString.Split(' '));

        for (int i = codeStringSplit.Count - 1; i >= 0; i--)
        {
            if (codeStringSplit[i] == code)
            {
                codeStringSplit.RemoveAt(i);
                break;
            }
        }

        CompileAndSaveNewString(codeStringSplit);
        //Debug.Log("Deleted Code new string = " + PlayerPrefs.GetString(PrefCodeMemory, ""));
    }

    public static void UpdatePrefsString(string code)
    {
        string codeString = PlayerPrefs.GetString(PrefCodeMemory, "");

        if (codeString.Contains(code))
            return;

        List<string> codeStringSplit = new List<string>(codeString.Split(' '));

        if (codeStringSplit.Count >= CodeSaveLimit)
        {
            codeStringSplit.RemoveAt(codeStringSplit.Count - 1);
            codeStringSplit.Insert(0, code);
            CompileAndSaveNewString(codeStringSplit);
        }
        else
        {
            PlayerPrefs.SetString(PrefCodeMemory, codeString == "" ? code : codeString + " " + code);
        }
    }

    private static void CompileAndSaveNewString(List<string> codeStringSplit)
    {
        string newPrefsString = string.Empty;

        for (int i = 0; i < codeStringSplit.Count; i++)
        {
            //Debug.Log("Code String = " + codeStringSplit[i]);
            newPrefsString += i == 0 ? codeStringSplit[i] : " " + codeStringSplit[i];
        }

        //Debug.Log("10 codes reached old string = " + codeString + " new string = " + newPrefsString);
        PlayerPrefs.SetString(PrefCodeMemory, newPrefsString.ToLower());
    }
}
