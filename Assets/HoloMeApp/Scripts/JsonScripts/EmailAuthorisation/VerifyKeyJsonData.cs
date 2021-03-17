using System;

[Serializable]
public class VerifyKeyJsonData
{
    public string key;

    public VerifyKeyJsonData() { }

    public VerifyKeyJsonData(string key) {
        this.key = key;
    }
}
