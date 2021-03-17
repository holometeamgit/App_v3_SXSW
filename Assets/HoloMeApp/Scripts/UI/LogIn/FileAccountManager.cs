using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem;

public class FileAccountManager : MonoBehaviour {
    public static string FacebookTokenFileName = "ftfn";
    public static string ServerAccessToken = "sat";

    #region save/load/remove function

    public static void SaveFile<T>(string key, T data, string fileName) {
//        Debug.Log("SaveFile " + key + " " + fileName);
        Cryptography cryptography = new Cryptography(key);
        string encrypted = JsonUtility.ToJson(data);//cryptography.Encrypt(data); //TODO comeback to cryptography

        FileSave fileSave = new FileSave(FileFormat.Binary);
        fileSave.WriteToFile(Application.persistentDataPath + "/" + fileName, encrypted);

//        Debug.Log(Application.persistentDataPath + "/" + fileName);
    }

    public static T ReadFile<T>(string key, string fileName) {
//        Debug.Log("ReadFile " + key + " " + fileName);
        T resultData = default;

        try {
            FileSave fileSave = new FileSave(FileFormat.Binary);
            string encrypted = fileSave.ReadFromFile<string>(Application.persistentDataPath + "/" + fileName, null);
  //          Debug.Log("ReadFile result " + encrypted);
            if (encrypted == null)
                return resultData;

            Cryptography cryptography = new Cryptography(key);
            resultData = JsonUtility.FromJson<T>(encrypted); //cryptography.Decrypt<T>(encrypted); //TODO comeback to cryptography
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
            return resultData;
        }

        return resultData;
    }

    public static void DeleteFile(string fileName) {
//        Debug.Log("DeleteFile " + fileName);
        FileSave fileSave = new FileSave(FileFormat.Binary);
        fileSave.DeleteFile(Application.persistentDataPath + "/" + fileName);
    }

    #endregion
}
