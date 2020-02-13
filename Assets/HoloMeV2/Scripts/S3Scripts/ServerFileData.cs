using System;

public class ServerFileData
{
    public string FileName { get; private set; }

    //public string ID { get; private set; }

    public DateTime LastModified { get; set; }

    public ServerFileData(string fileName, string ID, DateTime lastModified)
    {
        FileName = fileName;
        //this.ID = ID;
        LastModified = lastModified;
    }
}
