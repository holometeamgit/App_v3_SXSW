using System;

[Serializable]
public class ResetPasswordEmailJsonData
{
    public string email;

    public ResetPasswordEmailJsonData() { }

    public ResetPasswordEmailJsonData(string email) {
        this.email = email;
    }
}
