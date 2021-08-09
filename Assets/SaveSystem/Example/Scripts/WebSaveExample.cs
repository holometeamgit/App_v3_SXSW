using SaveSystem;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WebSaveExample : MonoBehaviour {

    //Change these values to make it work
    private const string SERVER = "www.myserver.com";
    private const string SERVER_PASSWORD = "password-for-my-server";
    private const string SERVER_USERNAME = "username-for-my-server";


    private void Start()
    {
        string localFilePath = Application.persistentDataPath + "/myFile.xml";

        #region SAVING DATA TO FILE
        List<MyClass> list = new List<MyClass>();
        for (int i = 0; i < 10; i++)
            list.Add(new MyClass());

        FileSave fileSave = new FileSave(FileFormat.Xml);
        fileSave.WriteToFile(localFilePath, list);
        #endregion

        NetworkCredential networkCredential = new NetworkCredential(SERVER_USERNAME, SERVER_PASSWORD, SERVER);
        int timeoutMilliseconds = 8000;

        //Creates a new WebSave object
        WebSave webSave = new WebSave(networkCredential, timeoutMilliseconds);

        //Is the server able to respond in the next 8000 milliseconds
        bool isConnected = webSave.IsConnected();

        if (isConnected)
        {
            //The time (in milliseconds) it takes the server to respond
            int ping = webSave.GetPingMilliseconds();

            //Everytime the progress changes the function OnProgress(float progress) is called.  
            WebSave.ProgressAction progressAction = OnProgress;

            //If the directory SaveExample doesn't exist
            if (!webSave.DirectoryExists("SaveExample"))
            {
                //Create new directories
                webSave.CreateDirectory("SaveExample", "");
            }

            //Upload the above created file. 
            currentProgress = 0;
            webSave.UploadFile(localFilePath, "SaveExample/testfile.xml", progressAction);

            //Upload the above created file. 
            currentProgress = 0;
            webSave.DowloadFile(localFilePath, "SaveExample/testfile.xml", progressAction);

            //Does a file exist
            bool fileExists = webSave.FileExists("SaveExample/testfile.xml");

            if(fileExists)
            {
                //Gets the last modified date of the file.


                //Renames file

                webSave.Rename("SaveExample/testfile.xml", "testfile.txt");
                //Renames direcory
                webSave.Rename("SaveExample", "Files");

                List<string> filesInDirectory = webSave.ListDirectory("Files", "");

                //Deletes the file
                webSave.DeleteFile("Files/testfile.txt");
            }
        }
    }

    #region UPLOAD/DOWNLOAD PROGRESS
    private int currentProgress;
    private void OnProgress(float progress)
    {
        if((int)(progress * 100) != currentProgress)
        {
            currentProgress = (int)(progress * 100);
        }
        if (progress == 1) { }
    }
    #endregion
}
