using System;

[Serializable]
public class ResendVerifyJsonData
{
    public string email;

    public ResendVerifyJsonData() { }

    public ResendVerifyJsonData(string email) {
        this.email = email;
    }
}
